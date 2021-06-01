using Microsoft.AspNetCore.Builder;
using Our.Umbraco.GraphQL.Web.Middleware;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLPipelineFilter : UmbracoPipelineFilter
    {
        public GraphQLPipelineFilter() : base(nameof(GraphQLPipelineFilter))
        {
            PostPipeline = a => a.UseMiddleware<GraphQLMiddleware>();
        }
    }
}
