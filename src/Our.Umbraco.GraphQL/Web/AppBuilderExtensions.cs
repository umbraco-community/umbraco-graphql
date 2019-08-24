using GraphQL;
using LightInject;
using Microsoft.Owin.Cors;
using Our.Umbraco.GraphQL.Web.Middleware;
using Owin;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Web
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app, string rootPath, IFactory factory)
        {
            return UseUmbracoGraphQL(app, rootPath, factory, new GraphQLServerOptions());
        }

        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app, string rootPath, IFactory factory, GraphQLServerOptions options)
        {
            string graphQLPath = $"/{rootPath}/graphql";

            return app.Map(graphQLPath, subApp =>
                {
                    var corsOptions = new CorsOptions
                    {
                        PolicyProvider = options.CorsPolicyProvder
                    };

                    subApp.UseCors(corsOptions)
                        .Use<FactoryMiddleware>(factory)
                        .Use((ctx, next) => options.EnablePlayground ? ctx.Get<IFactory>("umbraco:factory").CreateInstance<GraphQLPlaygroundMiddleware>(options).Invoke(ctx, next) : next())
                        .Use((ctx, next) => ctx.Get<IFactory>("umbraco:factory").GetInstance<GraphQLRequestParserMiddleware>().Invoke(ctx, next))
                        .Use((ctx, next) => ctx.Get<IFactory>("umbraco:factory").GetInstance<GraphQLMiddleware>().Invoke(ctx, options));
                });
        }
    }
}
