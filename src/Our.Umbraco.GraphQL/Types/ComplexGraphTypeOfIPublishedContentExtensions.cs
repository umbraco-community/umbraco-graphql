using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.ValueResolvers;
using System;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    internal static class ComplexGraphTypeOfIPublishedContentExtensions
    {
        public static ComplexGraphType<IPublishedContent> AddUmbracoContentPropeties(
            this ComplexGraphType<IPublishedContent> graphType,
            IContentTypeComposition contentType,
            PublishedItemType publishedItemType)
        {

            var publishedContentType = PublishedContentType.Get(publishedItemType, contentType.Alias);
            foreach (var property in contentType.CompositionPropertyTypes)
            {
                //TODO: black/whitelist properties
                if (property.PropertyEditorAlias == global::Umbraco.Core.Constants.PropertyEditors.ListViewAlias ||
                    property.PropertyEditorAlias == global::Umbraco.Core.Constants.PropertyEditors.FolderBrowserAlias ||
                    property.Alias.StartsWith("umbracoMember"))
                {
                    continue;
                }

                var publishedPropertyType = publishedContentType.GetPropertyType(property.Alias);

                var resolver = GetValueResolver(contentType, publishedPropertyType);

                var propertyGraphType = resolver.GetGraphQLType(publishedPropertyType);

                if (property.Mandatory)
                {
                    propertyGraphType = typeof(NonNullGraphType<>).MakeGenericType(propertyGraphType);
                }

                graphType.Field(
                    propertyGraphType,
                    property.Alias.ToCamelCase(),
                    property.Description,
                    resolve: context =>
                    {
                        var publishedProperty = context.Source.GetProperty(property.Alias);
                        return publishedProperty == null
                            ? null
                            : resolver.Resolve(publishedPropertyType, publishedProperty.Value);
                    }
                ).SetPermissions(graphType);
            }

            return graphType;
        }

        private static IGraphQLValueResolver GetValueResolver(IContentTypeComposition contentType, PublishedPropertyType propertyType)
        {
            var foundResolvers = GraphQLValueResolversResolver.Current.Resolvers.Where(r => r.IsResolver(propertyType)).ToList();
            var defaultResolvers = GraphQLValueResolversResolver.Current.DefaultResolvers;

            if(foundResolvers.Count == 1)
            {
                return foundResolvers[0];
            }

            if (foundResolvers.Count > 0)
            {
                //more than one resolver was found
                //get the non-default and see if we have only one
                var nonDefault = foundResolvers.Except(defaultResolvers.Select(x => x.Item1)).ToList();
                if (nonDefault.Count == 1)
                {
                    //there's only 1 custom resolver registered, so use it
                    return nonDefault[0];
                }

                //this is not allowed, there cannot be more than 1 resolver
                throw new InvalidOperationException($"Type '{nonDefault[1].GetType().FullName}' cannot be an IGraphQLValueResolver" +
                    $" for property '{propertyType.PropertyTypeAlias}' of content type '{contentType.Alias}' because type '{nonDefault[0].GetType().FullName}' has already been detected as a resolver" +
                    $" for that property, and only one converter can exist for a resolver.");
            }

            //no resolvers registered so we use the fallback resolver
            return GraphQLValueResolversResolver.Current.FallbackResolver;
        }
    }
}
