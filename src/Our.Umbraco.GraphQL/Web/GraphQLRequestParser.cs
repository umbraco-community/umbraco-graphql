using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLRequestParser
    {
        private readonly JsonSerializer _serializer;

        public GraphQLRequestParser(JsonSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<GraphQLRequest> Parse(HttpRequest request)
        {
            switch (request.ContentType)
            {
                case "application/json":
                    return DeserializeJson(request.Body);
                case "application/graphql":
                    return new GraphQLRequest(new GraphQLQuery
                    {
                        Query = await ReadAsStringAsync(request.Body).ConfigureAwait(false)
                    });
                default:
                    return null;
            }
        }

        private GraphQLRequest DeserializeJson(Stream stream)
        {
            var reader = new StreamReader(stream);
            using (var jsonTextReader = new JsonTextReader(reader) {CloseInput = false})
            {
                if (reader.Peek() == '[')
                {
                    return new GraphQLRequest(_serializer.Deserialize<GraphQLQuery[]>(jsonTextReader));
                }
                if (reader.Peek() == '{')
                {
                    return new GraphQLRequest(_serializer.Deserialize<GraphQLQuery>(jsonTextReader));
                }
            }

            return null;
        }

        private static async Task<string> ReadAsStringAsync(Stream stream) =>
            await new StreamReader(stream).ReadToEndAsync().ConfigureAwait(false);
    }
}
