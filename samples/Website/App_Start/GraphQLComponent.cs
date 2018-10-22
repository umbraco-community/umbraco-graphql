using LightInject;
using Our.Umbraco.GraphQL;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Core.Components;
using Umbraco.Core.Configuration;
using Umbraco.Web;

namespace Website.App_Start
{
    public class GraphQComponent : UmbracoComponentBase
    {
        public override void Compose(Composition composition)
        {
            composition.RegisterGraphQLServices();
        }

        public void Initialize(IGlobalSettings globalSettings, IServiceContainer container)
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += (s, e) =>
            {
                e.AppBuilder.UseUmbracoGraphQL(globalSettings.GetUmbracoMvcArea(), container, new GraphQLServerOptions
                {
                    EnableMetrics = true,
                    Debug = true,
                });
            };
        }
    }
}
