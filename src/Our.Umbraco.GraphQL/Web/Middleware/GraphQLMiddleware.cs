using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.GraphQL.Builders;
using Our.Umbraco.GraphQL.Types;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly IDocumentWriter _documentWriter;
        private readonly ILogger<GraphQLMiddleware> _logger;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IGraphQLRequestDeserializer _graphQLRequestDeserializer;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GraphQLMiddleware(RequestDelegate next,
                                 IDocumentExecuter documentExecuter,
                                 IDocumentWriter documentWriter,
                                 ILogger<GraphQLMiddleware> logger,
                                 ISchemaBuilder schemaBuilder,
                                 IGraphQLRequestDeserializer graphQLRequestDeserializer)
        {
            _next = next;
            _documentExecuter = documentExecuter ?? throw new ArgumentNullException(nameof(documentExecuter));
            _documentWriter = documentWriter ?? throw new ArgumentNullException(nameof(documentWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schemaBuilder = schemaBuilder;
            _graphQLRequestDeserializer = graphQLRequestDeserializer;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task Invoke(HttpContext context,
                                 IOptionsSnapshot<GraphQLServerOptions> graphQLServerOptions,
                                 ISchema schema)
        {
            var options = graphQLServerOptions.Value;

            if (!IsGraphQlRequest(context, options))
            {
                await _next(context);
                return;
            }

            if (!string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                await new PlaygroundMiddleware(_next, options.Playground).Invoke(context);
                return;
            }

            try
            {
                var deserialized = await _graphQLRequestDeserializer.DeserializeFromJsonBodyAsync(context.Request, context.RequestAborted);
                if (!deserialized.IsSuccessful)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var requests = (deserialized.Batch ?? new GraphQLRequest[] { deserialized.Single }).Select(x => Execute(schema, options, x, context.RequestAborted)).ToArray();
                var results = await Task.WhenAll(requests).ConfigureAwait(false);

                context.Response.ContentType = "application/json";
                if (deserialized.Batch != null) await _documentWriter.WriteAsync(context.Response.Body, results, context.RequestAborted).ConfigureAwait(false);
                else await _documentWriter.WriteAsync(context.Response.Body, results.First(), context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not handle GraphQL request");

                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error", context.RequestAborted).ConfigureAwait(false);
            }
        }

        private async Task<ExecutionResult> Execute(ISchema schema, GraphQLServerOptions options, GraphQLRequest request, CancellationToken cancellationToken)
        {
            var result = await _documentExecuter.ExecuteAsync(opts =>
            {
                opts.Schema = schema;
                opts.Query = request?.Query;
                opts.OperationName = request?.OperationName;
                opts.Inputs = request?.Inputs;
                opts.ValidationRules = DocumentValidator.CoreRules;
                opts.EnableMetrics = options.EnableMetrics;
                opts.CancellationToken = cancellationToken;
                opts.ComplexityConfiguration = options.Complexity;
            }).ConfigureAwait(false);

            if (result.Errors != null && result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.GetBaseException(), "There was an error" + (error.Path == null ? "" : " at [" + string.Join(", ", error.Path) + "]"));
                }
            }

            return result;
        }

        private static bool IsGraphQlRequest(HttpContext context, GraphQLServerOptions options) =>
            context.Request.Path.StartsWithSegments(options.Path)
            && (
                string.Equals(context.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase)
                || (
                    options.EnablePlayground
                    && string.Equals(context.Request.Method, "GET", StringComparison.InvariantCultureIgnoreCase)
                )
            );
    }
}
