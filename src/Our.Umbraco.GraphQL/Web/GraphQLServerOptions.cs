using System.Collections.Generic;
using System.Web.Cors;
using GraphQL.Validation.Complexity;
using Microsoft.Owin.Cors;
using Task = System.Threading.Tasks.Task;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLServerOptions
    {

        public GraphQLServerOptions()
        {
            SetCorsPolicy(new CorsPolicy
            {
                Headers =
                {
                    "X-Requested-With",
                    "X-HTTP-Method-Override",
                    "Content-Type",
                    "Cache-Control",
                    "Accept"
                },
                AllowAnyOrigin = true,
                Methods = {"POST"}
            });
        }

        public ICorsPolicyProvider CorsPolicyProvider { get; set; }
        public ComplexityConfiguration ComplexityConfiguration { get; set; }
        public bool Debug { get; set; }
        public bool EnableMetrics { get; set; }
        public bool EnableMiniProfiler { get; set; }
        public bool EnablePlayground { get; set; }
        public Dictionary<string, object> GraphQLConfig { get; set; }
        public Dictionary<string, object> PlaygroundSettings { get; set; }

        public void SetCorsPolicy(CorsPolicy policy)
        {
            CorsPolicyProvider = new CorsPolicyProvider
            {
                PolicyResolver = _ => Task.FromResult(policy)
            };
        }
    }
}
