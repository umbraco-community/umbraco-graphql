using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Our.Umbraco.GraphQL.Web
{
    internal class GraphQLRequestParserMiddleware : OwinMiddleware
    {
        public GraphQLRequestParserMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Method == "POST" &&
                context.Request.ContentType == "application/json" &&
                context.Request.Body.Length >= 0)
            {
                var stream = new StreamReader(context.Request.Body);
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
                            new[] {JsonConvert.DeserializeObject<GraphQLQuery>(stringContent)},
                            false
                        )
                    );
                }
            }
            if (Next != null)
            {
                await Next.Invoke(context);
            }
        }
    }
}