using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class GraphQLRequestParserMiddleware
    {
        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            if (context.Request.Method == "POST" &&
                context.Request.ContentType == "application/json" &&
                context.Request.Body.Length >= 0)
            {
                using (var stream = new StreamReader(context.Request.Body))
                {
                    var stringContent = await stream.ReadToEndAsync();
                    if (stringContent[0] == '[')
                    {
                        context.Set("Our.Umbraco.GraphQL::Request",
                            new GraphQLRequest(
                                JsonConvert.DeserializeObject<GraphQLQuery[]>(stringContent),
                                true
                            )
                        );
                    }
                    else if (stringContent[0] == '{')
                    {
                        context.Set("Our.Umbraco.GraphQL::Request",
                            new GraphQLRequest(
                                new[] { JsonConvert.DeserializeObject<GraphQLQuery>(stringContent) },
                                false
                            )
                        );
                    }
                }
            }
            if (next != null)
            {
                await next();
            }
        }
    }
}