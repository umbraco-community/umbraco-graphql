using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conversion;
using GraphQL.Http;
using GraphQL.Utilities;
using Microsoft.Owin;
using Our.Umbraco.GraphQL.Schema;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLMiddleware : OwinMiddleware
    {
        private readonly ApplicationContext _applicationContext;
        private readonly GraphQLServerOptions _options;

        public GraphQLMiddleware(
            OwinMiddleware next,
            ApplicationContext applicationContext,
            GraphQLServerOptions options) : base(next)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override async Task Invoke(IOwinContext context)
        {
            Stopwatch stopwatch = null;
            if (_options.Debug)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }

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
                   
                    IEnumerable<Task<ExecutionResult>> requests = request.Select(requestParams =>
                    {
                        try
                        {
                            string query = requestParams.Query;
                            string operationName = requestParams.OperationName;
                            Inputs variables = requestParams.Variables;
                            
                            return new DocumentExecuter()
                                .ExecuteAsync(x =>
                                {
                                    x.CancellationToken = context.Request.CallCancelled;
                                    //x.ComplexityConfiguration = new ComplexityConfiguration();
                                    x.ExposeExceptions = _options.Debug;
                                    //x.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
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
                                        _options
                                    );
                                });
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });

                    var responses = await Task.WhenAll(requests);

                    context.Response.ContentType = "application/json";

                    var writer = new DocumentWriter();
                    if (false == request.IsBatched)
                    {
                        var response = writer.Write(responses[0]);
                        await context.Response.WriteAsync(response);
                    }
                    else
                    {
                        var response = writer.Write(responses);
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
            if (_options.Debug)
            {
                stopwatch.Stop();
                context.Response.Headers.Append("took", stopwatch.ElapsedMilliseconds.ToString());
            }
        }
    }
}
