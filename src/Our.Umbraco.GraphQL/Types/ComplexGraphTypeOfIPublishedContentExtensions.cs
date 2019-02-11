using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.ValueResolvers;
using System;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Types
{
    internal static class ComplexGraphTypeOfIPublishedContentExtensions
    {
        public static ComplexGraphType<IPublishedContent> AddUmbracoBuiltInProperties(this ComplexGraphType<IPublishedContent> graphType)
        {
            //TODO: black/whitelist properties
            graphType.Field<NonNullGraphType<DateGraphType>>()
                .Name("_createDate")
                .Description("Create date of the content.")
                .Resolve(context => context.Source.Id)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_creatorName")
                .Description("Name of the content creator.")
                .Resolve(context => context.Source.CreatorName)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_documentTypeAlias")
                .Description("Document type alias of the content.")
                .Resolve(context => context.Source.DocumentTypeAlias)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<IdGraphType>>()
                .Name("_id")
                .Description("Unique id of the content.")
                .Resolve(context => context.Source.Id)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<IntGraphType>>()
                .Name("_index")
                .Description("Index of the content.")
                .Resolve(context => context.Source.GetIndex())
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<IntGraphType>>()
                .Name("_level")
                .Description("Level of the content.")
                .Resolve(context => context.Source.Level)
                .SetPermissions(graphType);

            graphType.Field<NonNullGraphType<BooleanGraphType>>()
                .Name("_isFirst")
                .Description("Is the content first in the list.")
                .Resolve(context => context.Source.IsFirst())
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<BooleanGraphType>>()
                .Name("_isLast")
                .Description("Is the content last in the list.")
                .Resolve(context => context.Source.IsLast())
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<BooleanGraphType>>()
                .Name("_isVisible")
                .Description("Is the content visible.")
                .Resolve(context => context.Source.IsVisible())
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_name")
                .Description("Name of the content.")
                .Resolve(context => context.Source.Name)
                .SetPermissions(graphType, true);

            graphType.Field<PublishedContentInterfaceGraphType>()
                .Name("_parent")
                .Description("Parent of the content.")
                .Resolve(context => context.Source.Parent)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<IntGraphType>>()
                .Name("_sortOrder")
                .Description("SortOrder of the content.")
                .Resolve(context => context.Source.SortOrder)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<DateGraphType>>()
                .Name("_updateDate")
                .Description("Update date of the content.")
                .Resolve(context => context.Source.UpdateDate)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_url")
                .Description("Url of the content.")
                .Resolve(context => context.Source.Url)
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_urlAbsolute")
                .Description("Absolute url of the content.")
                .Resolve(context => context.Source.UrlAbsolute())
                .SetPermissions(graphType, true);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_writerName")
                .Description("Name of the content writer.")
                .Resolve(context => context.Source.WriterName)
                .SetPermissions(graphType, true);

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("_ancestors")
                .Description("Ancestors of the content.")
                .Argument<BooleanGraphType>("includeSelf", "include self in list")
                .Bidirectional()
                .Resolve(context =>
                    (context.GetArgument<bool?>("includeSelf") == true
                        ? context.Source.AncestorsOrSelf()
                        : context.Source.Ancestors()).Filter(context).ToConnection(context)
                );

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("_siblings")
                .Description("Siblings of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Siblings().Filter(context).ToConnection(context));

            graphType.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("_children")
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

            if (foundResolvers.Count == 1)
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
