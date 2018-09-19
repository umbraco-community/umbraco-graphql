using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conversion;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Utilities;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Our.Umbraco.GraphQL.Instrumentation;
using Our.Umbraco.GraphQL.Schema;
using StackExchange.Profiling;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLMiddleware : OwinMiddleware
    {
        private readonly ApplicationContext _applicationContext;
        private readonly GraphQLServerOptions _options;
        private readonly DocumentExecuter _documentExecutor;
        private readonly DocumentWriter _documentWriter;

        public GraphQLMiddleware(
            OwinMiddleware next,
            ApplicationContext applicationContext,
            GraphQLServerOptions options) : base(next)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _documentExecutor = new DocumentExecuter();
            _documentWriter = new DocumentWriter();
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                var schema = _applicationContext.ApplicationCache.RuntimeCache.GetCacheItem<UmbracoSchema>(
                    "Our.Umbraco.GraphQL::Schema",
                    () =>

                        new UmbracoSchema(
                            _applicationContext.Services.ContentTypeService,
                            _applicationContext.Services.MemberTypeService,
                            _options
                        )
                );

                if (false == context.Request.Path.HasValue)
                {
                    var request = context.Get<GraphQLRequest>("Our.Umbraco.GraphQL::Request");
                    switch (context.Request.Method)
                    {
                        case "POST":
                            if (request == null)
                            {
                                context.Response.StatusCode = 400;
                                await context.Response.WriteAsync("POST body missing.");
                                return;
                            }
                            break;
                        default:
                            context.Response.StatusCode = 405;
                            await context.Response.WriteAsync("Server supports only POST requests.");
                            return;
                    }

                    IEnumerable<Task<ExecutionResult>> requests = request.Select(async requestParams =>
                    {
                        string query = requestParams.Query;
                        string operationName = requestParams.OperationName;
                        string accessToken = context.Request.Query["accessToken"];
                        Inputs variables = requestParams.Variables;

                        var start = DateTime.Now;
                        MiniProfiler.Start();
                        var errors = new ExecutionErrors();
                        var result = await _documentExecutor
                            .ExecuteAsync(x =>
                            {
                                x.CancellationToken = context.Request.CallCancelled;
                                //x.ComplexityConfiguration = new ComplexityConfiguration();
                                x.ExposeExceptions = _options.Debug;
                                if (_options.EnableMetrics)
                                {
                                    x.EnableMetrics = true;
                                    x.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                                    x.FieldMiddleware.Use<MiniProfilerFieldsMiddleware>();
                                }
                                x.FieldNameConverter = new DefaultFieldNameConverter();
                                x.Inputs = variables;
                                x.OperationName = operationName;
                                x.Query = query;
                                //x.Root = 
                                x.Schema = schema;
                                x.UserContext = new UmbracoGraphQLContext(
                                    context.Request.Uri,
                                    _applicationContext,
                                    UmbracoContext.Current,
                                    _options,
                                    accessToken,
                                    out errors
                                );
                            });

                        // Save any of our errors reported by our authentication stuff in UserContext
                        if (errors.Any())
                        {
                            if (result.Errors != null)
                            {
                                result.Errors.Concat(errors);
                            }
                            else
                            {
                                result.Errors = errors;
                            }
                        }

                        if (_options.EnableMetrics && result.Errors == null)
                        {
                            result.EnrichWithApolloTracing(start);

                            if (result.Extensions == null)
                            {
                                result.Extensions = new Dictionary<string, object>();
                            }
                            result.Extensions["miniProfiler"] = JObject.FromObject(MiniProfiler.Current, new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                        }
                        MiniProfiler.Stop();

                        return result;
                    });

                    var responses = await Task.WhenAll(requests);

                    context.Response.ContentType = "application/json";

                    if (false == request.IsBatched)
                    {
                        var response = _documentWriter.Write(responses[0]);
                        await context.Response.WriteAsync(response);
                    }
                    else
                    {
                        var response = _documentWriter.Write(responses);
                        await context.Response.WriteAsync(response);
                    }
                }
                else if (context.Request.Path.ToString() == "/schema")
                {
                    using (var schemaPrinter = new SchemaPrinter(schema))
                    {
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(schemaPrinter.Print());
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 500;

                if (_options.Debug)
                {
                    await context.Response.WriteAsync(ex.ToString());
                }
                else
                {
                    await context.Response.WriteAsync("Internal server error");
                }
            }
        }
    }
}
