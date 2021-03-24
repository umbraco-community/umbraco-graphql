using System;
using Microsoft.AspNetCore.Builder;
using Our.Umbraco.GraphQL.Web.Middleware;
using Umbraco.Extensions;

namespace Our.Umbraco.GraphQL.Web
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseUmbracoGraphQL(this IApplicationBuilder app, string path, IServiceProvider serviceProvider, Action<GraphQLServerOptions> configure = null)
        {
            var options = new GraphQLServerOptions();
            configure?.Invoke(options);

            return app.Map(path, subApp =>
                {
                    subApp.UseCors(options.CorsPolicyName)
                        .Use((ctx, next) => options.EnablePlayground ? serviceProvider.CreateInstance<GraphQLPlaygroundMiddleware>(options).Invoke(ctx, next) : next())
                        .Use((ctx, next) => serviceProvider.CreateInstance<GraphQLMiddleware>(options).Invoke(ctx, next));
                });
        }
    }
}
