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
        public static ComplexGraphType<IPublishedContent> AddUmbracoBuiltInProperties(this ComplexGraphType<IPublishedContent> graphType)
        {
            // TODO: set this field name as a reserved property alias
            graphType.Field<PublishedContentDataGraphType>("_contentData", "Built in published content data.", resolve: context => context.Source).SetDoctypeMetadata(graphType.GetMetadata<string>("documentTypeAlias"));

            return graphType;
        }

        public static ComplexGraphType<IPublishedContent> AddContentDataProperties(this ComplexGraphType<IPublishedContent> graphType)
        {
            //TODO: black/whitelist properties
            graphType.Field<NonNullGraphType<DateGraphType>>("createDate", "Create date of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("creatorName", "Name of the content creator.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("documentTypeAlias", "Document type alias of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<IdGraphType>>("id", "Unique id of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<IntGraphType>>("index", "Index of the content.", resolve: context => context.Source.GetIndex()).SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<IntGraphType>>("level", "Level of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isFirst", "Is the content first in the list.", resolve: context => context.Source.IsFirst()).SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isLast", "Is the content last in the list.", resolve: context => context.Source.IsLast()).SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isVisible", "Is the content visible.", resolve: context => context.Source.IsVisible()).SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("name", "Name of the content.").SetPermissions(graphType, true);
            graphType.Field<PublishedContentInterfaceGraphType>("parent", "Parent of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<IntGraphType>>("sortOrder", "SortOrder of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<DateGraphType>>("updateDate", "Update date of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("url", "Url of the content.").SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("urlAbsolute", "Absolute url of the content.", resolve: context => context.Source.UrlAbsolute()).SetPermissions(graphType, true);
            graphType.Field<NonNullGraphType<StringGraphType>>("writerName", "Name of the content writer.").SetPermissions(graphType, true);

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("ancestors")
                .Description("Ancestors of the content.")
                .Argument<BooleanGraphType>("includeSelf", "include self in list")
                .Bidirectional()
                .Resolve(context =>
                    (context.GetArgument<bool?>("includeSelf") == true
                        ? context.Source.AncestorsOrSelf()
                        : context.Source.Ancestors()).Filter(context).ToConnection(context)
                );

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("siblings")
                .Description("Siblings of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Siblings().Filter(context).ToConnection(context));

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("children")
                .Description("Children of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Children.Filter(context).ToConnection(context));

            return graphType;
        }

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
