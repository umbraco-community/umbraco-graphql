using GraphQL.Server.Ui.Playground;
using GraphQL.Validation.Complexity;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLServerOptions
    {
        public string Path { get; set; } = "/umbraco/graphql";
        public bool EnableMetrics { get; set; }
        public bool EnablePlayground { get; set; }
        public bool EnableCors { get; set; }
        public string CorsPolicyName { get; set; }
        public string[] CorsAllowedOrigins { get; set; }
        public string[] CorsAllowedExposedHeaders { get; set; }
        public string[] CorsAllowedHeaders { get; set; }
        public string[] CorsAllowedMethods { get; set; }
        public ComplexityConfiguration Complexity { get; set; } = new ComplexityConfiguration();
        public PlaygroundOptions Playground { get; set; } = new PlaygroundOptions { GraphQLEndPoint = "/umbraco/graphql", SubscriptionsEndPoint = "/umbraco/graphql" };
    }
}
