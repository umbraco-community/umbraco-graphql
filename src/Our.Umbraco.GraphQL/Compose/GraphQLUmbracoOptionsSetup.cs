using Microsoft.Extensions.Options;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLUmbracoOptionsSetup : IConfigureOptions<UmbracoPipelineOptions>
    {
        private readonly IOptions<GraphQLServerOptions> _options;

        public GraphQLUmbracoOptionsSetup(IOptions<GraphQLServerOptions> options)
        {
            _options = options;
        }

        public void Configure(UmbracoPipelineOptions options)
        {
            options.AddFilter(new GraphQLPipelineFilter(_options.Value));
        }
    }
}
