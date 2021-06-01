using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.GraphQL.Builders;
using Our.Umbraco.GraphQL.Types;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly IDocumentWriter _documentWriter;
        private readonly ILogger<GraphQLMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GraphQLMiddleware(RequestDelegate next,
                                 IDocumentExecuter documentExecuter,
                                 IDocumentWriter documentWriter,
                                 ILogger<GraphQLMiddleware> logger)
        {
            _next = next;
            _documentExecuter = documentExecuter ?? throw new ArgumentNullException(nameof(documentExecuter));
            _documentWriter = documentWriter ?? throw new ArgumentNullException(nameof(documentWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                var request = await JsonSerializer.DeserializeAsync<GraphQLRequest>(context.Request.Body, _jsonSerializerOptions, context.RequestAborted);
                var result = await _documentExecuter.ExecuteAsync(opts =>
                {
                    opts.Schema = schema;
                    opts.Query = request?.Query;
                    opts.OperationName = request?.OperationName;
                    opts.Inputs = request?.Inputs;
                    opts.ValidationRules = DocumentValidator.CoreRules;
                    opts.EnableMetrics = options.EnableMetrics;
                    opts.CancellationToken = context.RequestAborted;
                    opts.ComplexityConfiguration = options.Complexity;
                });

                if (request?.OperationName == "IntrospectionQuery" && result.Errors != null && result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError(error.GetBaseException(), "Could not introspect schema");
                    }
                }

                context.Response.ContentType = "application/json";
                await _documentWriter.WriteAsync(context.Response.Body, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not handle GraphQL request");

                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }
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
