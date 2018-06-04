using Umbraco.Core;
using Umbraco.Web;
using Our.Umbraco.GraphQL;
using Our.Umbraco.GraphQL.Web;

namespace Website.App_Start
{
    public class UmbracoEvents : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += (sender, e) => e.AppBuilder.UseUmbracoGraphQL(applicationContext);
        }
    }
}
