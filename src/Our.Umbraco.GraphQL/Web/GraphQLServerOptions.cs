using System.Collections.Generic;
using GraphQL.Validation.Complexity;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLServerOptions
    {
        public string CorsPolicyName { get; set; }
        public ComplexityConfiguration ComplexityConfiguration { get; set; }
        public bool Debug { get; set; }
        public bool EnableMetrics { get; set; }
        public bool EnableMiniProfiler { get; set; }
        public bool EnablePlayground { get; set; }
        public Dictionary<string, object> GraphQLConfig { get; set; }
        public Dictionary<string, object> PlaygroundSettings { get; set; }
    }
}
