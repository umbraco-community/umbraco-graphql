using System;
using Microsoft.Owin.Cors;
using Our.Umbraco.GraphQL.Web.Middleware;
using Owin;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Web
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app, string path, IFactory factory, Action<GraphQLServerOptions> configure = null)
        {
            var options = new GraphQLServerOptions();
            configure?.Invoke(options);

            return app.Map(path, subApp =>
                {
                    var corsOptions = new CorsOptions
                    {
                        PolicyProvider = options.CorsPolicyProvider
                    };

                    subApp.UseCors(corsOptions)
                        .Use<FactoryMiddleware>(factory)
                        .Use((ctx, next) => options.EnablePlayground ? ctx.Get<IFactory>(typeof(IFactory).AssemblyQualifiedName).CreateInstance<GraphQLPlaygroundMiddleware>(options).Invoke(ctx, next) : next())
                        .Use((ctx, next) => ctx.Get<IFactory>(typeof(IFactory).AssemblyQualifiedName).CreateInstance<GraphQLMiddleware>(options).Invoke(ctx, next));
                });
        }
    }
}
