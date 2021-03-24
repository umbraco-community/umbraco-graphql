using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLPlaygroundMiddleware
    {
        private static readonly string Html;
        private readonly GraphQLServerOptions _options;

        static GraphQLPlaygroundMiddleware()
        {
            using (var stream = typeof(AppBuilderExtensions).Assembly.GetManifestResourceStream("Our.Umbraco.GraphQL.Resources.playground.html"))
            {
                using (var reader = new StreamReader(stream))
                {
                    Html = reader.ReadToEnd();
                }
            }
        }

        public GraphQLPlaygroundMiddleware(GraphQLServerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Invoke(HttpContext context, Func<Task> next)
        {
            if (context.Request.Method == "GET")
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(
                    Html.Replace("{{GraphQLEndPoint}}", context.Request.Path)
                        .Replace("{{GraphQLConfig}}", JsonConvert.SerializeObject(_options.GraphQLConfig))
                        .Replace("{{PlaygroundSettings}}", JsonConvert.SerializeObject(_options.PlaygroundSettings))
                    );
            }
            else
            {
                await next();
            }
        }
    }
}
