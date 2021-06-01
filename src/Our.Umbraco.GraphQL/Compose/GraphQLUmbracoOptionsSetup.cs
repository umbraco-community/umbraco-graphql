using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLUmbracoOptionsSetup : IConfigureOptions<UmbracoPipelineOptions>
    {
        public GraphQLUmbracoOptionsSetup()
        {
        }

        public void Configure(UmbracoPipelineOptions options)
        {
            options.AddFilter(new GraphQLPipelineFilter());
        }
    }
}
