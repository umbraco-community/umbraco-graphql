using System;
using System.IO;
using Microsoft.Owin.Cors;
using Owin;
using umbraco;
using Umbraco.Core;

namespace Our.Umbraco.GraphQL.Web
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app)
        {
            return app.UseUmbracoGraphQL(ApplicationContext.Current);
        }

        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app, ApplicationContext applicationContext)
        {
            return app.UseUmbracoGraphQL(applicationContext, new GraphQLServerOptions());
        }

        public static IAppBuilder UseUmbracoGraphQL(this IAppBuilder app,
            ApplicationContext applicationContext, GraphQLServerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            //var jsonSerializerSettings = new JsonSerializerSettings
            //{
            //    DateFormatHandling = DateFormatHandling.IsoDateFormat,
            //    DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'",
            //    ContractResolver = new DefaultContractResolver()
            //};
            //var documentExecuter = new DocumentExecuter();
            //var documentWriter = new DocumentWriter(Formatting.None, jsonSerializerSettings);
            //var requestExecutor = new RequestExecutor(documentExecuter, documentWriter);

            //TODO: Make GraphiQL endpoint configurable
            var graphiQLPath = $"/{GlobalSettings.UmbracoMvcArea}/graphiql";
            var graphQLPath = $"/{GlobalSettings.UmbracoMvcArea}/graphql";


            return app.Map(graphiQLPath, subApp =>
            {
                string html;

                using (var stream = typeof(AppBuilderExtensions).Assembly.GetManifestResourceStream("Our.Umbraco.GraphQL.Resources.graphiql.html"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd().Replace("${endpointURL}", graphQLPath);
                    }
                }
                subApp.Run(async ctx =>
                {
                    ctx.Response.ContentType = "text/html";
                    await ctx.Response.WriteAsync(html);
                });
            })
            .Map(graphQLPath, subApp =>
            {
                var corsOptions = new CorsOptions
                {
                    PolicyProvider = options.CorsPolicyProvder
                };

                subApp.UseCors(corsOptions)
                    .Use<GraphQLRequestParserMiddleware>()
                    .Use<GraphQLMiddleware>(applicationContext, options);
            });
        }
    }
}
