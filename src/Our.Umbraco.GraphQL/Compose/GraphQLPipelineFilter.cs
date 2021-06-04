using Microsoft.AspNetCore.Builder;
using Our.Umbraco.GraphQL.Web;
using Our.Umbraco.GraphQL.Web.Middleware;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLPipelineFilter : UmbracoPipelineFilter
    {
        public GraphQLPipelineFilter(GraphQLServerOptions options) : base(nameof(GraphQLPipelineFilter))
        {
            PostPipeline = a =>
            {
                if (options.EnableCors && !string.IsNullOrWhiteSpace(options.CorsPolicyName))
                {
                    a.UseCors(options.CorsPolicyName);
                }
                a.UseMiddleware<GraphQLMiddleware>();
            };
        }
    }
}
