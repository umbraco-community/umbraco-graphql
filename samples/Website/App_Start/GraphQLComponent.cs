using System.Web.Hosting;
using Our.Umbraco.GraphQL.Web;
using Owin;
using Umbraco.Core.Configuration;
using Umbraco.Core.Composing;
using Umbraco.Core;
using Umbraco.Web;

namespace Website
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class GraphQLComposer : ComponentComposer<GraphQLComponent>, IUserComposer
    {
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
            UmbracoDefaultOwinStartup.MiddlewareConfigured += UmbracoDefaultOwinStartup_MiddlewareConfigured;
        }

        private void UmbracoDefaultOwinStartup_MiddlewareConfigured(object sender, OwinMiddlewareConfiguredEventArgs e) =>
            Configure(e.AppBuilder);

        private void Configure(IAppBuilder app)
        {
            var path = $"/{_globalSettings.GetUmbracoMvcArea()}/graphql";

            app.UseUmbracoGraphQL(path, _factory, opts =>
            {
                opts.Debug = HostingEnvironment.IsDevelopmentEnvironment;
                opts.EnableMetrics = true;
                opts.EnableMiniProfiler = false;
                opts.EnablePlayground = true;
            });
        }

        public void Terminate()
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured -= UmbracoDefaultOwinStartup_MiddlewareConfigured;
        }
    }
}
