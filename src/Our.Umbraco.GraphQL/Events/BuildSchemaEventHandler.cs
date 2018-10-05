using System;
using System.Web.Mvc;
using Our.Umbraco.GraphQL.ValueResolvers;
using Umbraco.Core;
using Umbraco.Web.Cache;

namespace Our.Umbraco.GraphQL
{
    internal class UmbracoEvents : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            GraphQLValueResolversResolver.Current = new GraphQLValueResolversResolver(
                new ServiceProvider(),
                applicationContext.ProfilingLogger.Logger,
                PluginManager.Current.ResolveTypes<IGraphQLValueResolver>()
            );

            DataTypeCacheRefresher.CacheUpdated += (s, e) => ClearSchemaCache(applicationContext);
            ContentTypeCacheRefresher.CacheUpdated += (s, e) => ClearSchemaCache(applicationContext);
        }

        private void ClearSchemaCache(ApplicationContext applicationContext)
        {
            applicationContext.ApplicationCache.RuntimeCache.ClearCacheItem("Our.Umbraco.GraphQL::Schema");
        }

        internal class ServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return DependencyResolver.Current.GetService(serviceType) ??
                       Activator.CreateInstance(serviceType);
            }
        }
    }
}
