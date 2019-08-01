using Microsoft.Owin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLPlaygroundMiddleware
    {
        private readonly string _html;

        public GraphQLPlaygroundMiddleware()
        {
            using (var stream = typeof(AppBuilderExtensions).Assembly.GetManifestResourceStream("Our.Umbraco.GraphQL.Resources.playground.html"))
            {
                using (var reader = new StreamReader(stream))
                {
                    _html = reader.ReadToEnd(); //.Replace("${endpointURL}", graphQLPath);
                }
            }
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            if (context.Request.Method == "GET")
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(_html);
            }
            else
            {
                await next();
            }
        }
    }
}
