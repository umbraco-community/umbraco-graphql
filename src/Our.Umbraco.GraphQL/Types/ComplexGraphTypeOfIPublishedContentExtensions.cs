using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.ValueResolvers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

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
                if (property.PropertyEditorAlias == Constants.PropertyEditors.ListViewAlias ||
                    property.PropertyEditorAlias == Constants.PropertyEditors.FolderBrowserAlias ||
                    property.Alias.StartsWith("umbracoMember"))
                {
                    continue;
                }

                var publishedPropertyType = publishedContentType.GetPropertyType(property.Alias);

                var resolver = GraphQLValueResolversResolver.Current.FindResolver(publishedPropertyType)
                               ?? new DefaultValueResolver();

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
    }
}
