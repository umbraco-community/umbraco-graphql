using GraphQL;
using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Json;
using Our.Umbraco.GraphQL.Json.Converters;

namespace Our.Umbraco.GraphQL.Web
{

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }

        [JsonConverter(typeof(InputsConverter))]
        public Inputs Variables { get; set; }
    }
}