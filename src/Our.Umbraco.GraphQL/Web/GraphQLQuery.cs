using GraphQL;
using Newtonsoft.Json;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }

        [JsonConverter(typeof(InputConverter))]
        public Inputs Variables { get; set; }
    }
}