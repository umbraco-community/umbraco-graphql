using GraphQL;
using GraphQL.Http;
using Our.Umbraco.GraphQL.ValueResolvers;
using Our.Umbraco.GraphQL.Web.Middleware;
using System;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL
{
    public static class CompositionExtensions
    {
        public static void RegisterGraphQLServices(this Composition composition)
        {
            composition.Register<ISchemaBuilder, UmbracoSchemaBuilder>(Lifetime.Singleton);
            composition.Register<IPublishedContentGraphTypeBuilder, PublishedContentGraphTypeBuilder>();

            composition.Register<IDependencyResolver>(factory => new FuncDependencyResolver(type => factory.TryGetInstance(type) ?? Activator.CreateInstance(type)));
            composition.Register<IDocumentExecuter, DocumentExecuter>(Lifetime.Singleton);
            composition.Register<IDocumentWriter, DocumentWriter>(Lifetime.Singleton);

            composition.Register<GraphQLRequestParserMiddleware>(Lifetime.Singleton);
            composition.Register<GraphQLMiddleware>(Lifetime.Singleton);

            composition.WithCollectionBuilder<GraphQLValueResolverCollectionBuilder>()
                .Append(composition.TypeLoader.GetTypes<IGraphQLValueResolver>());
        }
    }
}
