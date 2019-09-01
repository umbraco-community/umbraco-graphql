using GraphQL;
using GraphQL.Conversion;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Our.Umbraco.GraphQL.Instrumentation;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLMiddleware
    {
        private readonly IDocumentWriter _documentWriter;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ILocalizationService _localizationService;

        public GraphQLMiddleware(IDocumentExecuter documentExecuter, IDocumentWriter documentWriter, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService)
        {
              _umbracoContextAccessor = umbracoContextAccessor ?? throw new ArgumentNullException(nameof(umbracoContextAccessor));
            _documentExecuter = documentExecuter ?? throw new ArgumentNullException(nameof(documentExecuter));
            _documentWriter = documentWriter ?? throw new ArgumentNullException(nameof(documentWriter));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public string Culture { get; private set; }

        public async Task Invoke(IOwinContext context, GraphQLServerOptions options)
        {
            try
            {
                // TODO: Add ISchemaCacher
                using (ISchema schema = new Schema())
                {
                    if (context.Request.Path.HasValue)
                    {
                        if (context.Request.Path.ToString() == "/schema")
                        {
                            using (SchemaPrinter schemaPrinter = new SchemaPrinter(schema))
                            {
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync(schemaPrinter.Print());
                            }
                        }
                    }
                    else
                    {
                        GraphQLRequest request = context.Get<GraphQLRequest>("Our.Umbraco.GraphQL::Request");
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

                        List<ILanguage> allLanguages = _localizationService.GetAllLanguages().ToList();
                        UmbracoContext umbracoContext = _umbracoContextAccessor.UmbracoContext;

                        // TODO: Figure out a better way to handle cultures
                        context.Request.Headers.TryGetValue("Accept-Language", out string[] languageHeaders);
                        string[] languages = languageHeaders.FirstOrDefault()?.Split(';', ',');
                        string culture = languages?.FirstOrDefault(x => allLanguages.Any(l => l.CultureInfo.Name == x))
                            ?? allLanguages.FirstOrDefault(x => x.IsDefault)?.CultureInfo.Name
                            ?? allLanguages.FirstOrDefault()?.CultureInfo.Name
                            ?? CultureInfo.CurrentUICulture.Name;

                        IEnumerable<Task<ExecutionResult>> requests = request.Select(async requestParams =>
                        {
                            string query = requestParams.Query;
                            string operationName = requestParams.OperationName;
                            Inputs variables = requestParams.Variables;

                            DateTime start = DateTime.Now;
                            var miniProfiler = MiniProfiler.StartNew();
                            ExecutionResult result = await _documentExecuter
                                .ExecuteAsync(x =>
                                {
                                    x.CancellationToken = context.Request.CallCancelled;
                                    //x.ComplexityConfiguration = new ComplexityConfiguration();
                                    x.ExposeExceptions = options.Debug;
                                    if (options.EnableMetrics)
                                    {
                                        x.EnableMetrics = true;
                                        x.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                                        x.FieldMiddleware.Use<MiniProfilerFieldsMiddleware>();
                                    }
                                    x.ExposeExceptions = options.Debug;
                                    x.FieldNameConverter = new DefaultFieldNameConverter();
                                    x.Inputs = variables;
                                    x.OperationName = operationName;
                                    x.Query = query;
                                    x.Schema = schema;
                                    x.UserContext = new UmbracoGraphQLContext(
                                        context.Request.Uri,
                                        umbracoContext,
                                        culture
                                    );
                                });

                            if (options.EnableMetrics && result.Errors == null)
                            {
                                result.EnrichWithApolloTracing(start);

                                if (result.Extensions == null)
                                {
                                    result.Extensions = new Dictionary<string, object>();
                                }
                                result.Extensions["miniProfiler"] = JObject.FromObject(MiniProfiler.Current, new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                            }
                            miniProfiler.Stop();

                            return result;
                        });

                        ExecutionResult[] responses = await Task.WhenAll(requests);

                        context.Response.ContentType = "application/json";

                        if (false == request.IsBatched)
                        {
                            await _documentWriter.WriteAsync(context.Response.Body, responses[0]);
                        }
                        else
                        {
                            await _documentWriter.WriteAsync(context.Response.Body, responses);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 500;

                if (options.Debug)
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
