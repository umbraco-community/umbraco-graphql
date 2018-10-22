using GraphQL;
using GraphQL.Http;
using LightInject;
using Our.Umbraco.GraphQL.ValueResolvers;
using Our.Umbraco.GraphQL.Web.Middleware;
using System;
using Umbraco.Core.Components;
using global::Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL
{
    public static class CompositionExtensions
    {
        public static void RegisterGraphQLServices(this Composition composition)
        {
            composition.Container.RegisterSingleton<ISchemaBuilder, UmbracoSchemaBuilder>();
            composition.Container.RegisterSingleton<IPublishedContentGraphTypeBuilder, PublishedContentGraphTypeBuilder>();

            composition.Container.RegisterInstance<IDependencyResolver>(new FuncDependencyResolver(t => composition.Container.TryGetInstance(t) ?? Activator.CreateInstance(t)));
            composition.Container.RegisterSingleton<IDocumentExecuter, DocumentExecuter>();
            composition.Container.RegisterSingleton<IDocumentWriter, DocumentWriter>();

            composition.Container.RegisterSingleton<GraphQLRequestParserMiddleware>();
            composition.Container.RegisterSingleton<GraphQLMiddleware>();

            composition.Container.RegisterCollectionBuilder<GraphQLValueResolverCollectionBuilder>()
                .Append(factory => factory.GetInstance<TypeLoader>().GetTypes<IGraphQLValueResolver>());
        }
    }
}
