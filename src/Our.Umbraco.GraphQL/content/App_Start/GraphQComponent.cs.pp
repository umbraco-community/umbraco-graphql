using Our.Umbraco.GraphQL;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Core.Configuration;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Core;

namespace $rootnamespace$
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class GraphQLComposer : ComponentComposer<GraphQLComponent>, IUserComposer
    {
        public override void Compose(Composition composition)
        {
            base.Compose(composition);

            composition.RegisterGraphQLServices();
        }
    }

    public class GraphQLComponent : IComponent
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IFactory _factory;

        public GraphQLComponent(IGlobalSettings globalSettings, IFactory factory)
        {
            _globalSettings = globalSettings ?? throw new System.ArgumentNullException(nameof(globalSettings));
            _factory = factory ?? throw new System.ArgumentNullException(nameof(factory));
        }

        public void Initialize()
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += (s, e) =>
            {
                e.AppBuilder.UseUmbracoGraphQL(_globalSettings.GetUmbracoMvcArea(), _factory, new GraphQLServerOptions
                {
                    EnableMetrics = true,
                    Debug = true,
                });
            };
        }

        public void Terminate()
        {
        }
    }
}
