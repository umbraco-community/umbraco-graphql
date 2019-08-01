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
                .Resolve(context => context.Source.Id);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_creatorName")
                .Description("Name of the content creator.")
                .Resolve(context => context.Source.CreatorName);

            graphType.Field<NonNullGraphType<PublishedContentTypeGraphType>>()
                .Name("_contentType")
                .Description("Content type of the content.")
                .Resolve(context => context.Source.ContentType);

            graphType.Field<NonNullGraphType<IdGraphType>>()
                .Name("_id")
                .Description("Unique id of the content.")
                .Resolve(context => context.Source.Id);

            //graphType.Field<NonNullGraphType<IntGraphType>>()
            //    .Name("_index")
            //    .Description("Index of the content.")
            //    .Resolve(context => context.Source.GetIndex());

            graphType.Field<NonNullGraphType<IntGraphType>>()
                .Name("_level")
                .Description("Level of the content.")
                .Resolve(context => context.Source.Level);

            //graphType.Field<NonNullGraphType<BooleanGraphType>>()
            //    .Name("_isFirst")
            //    .Description("Is the content first in the list.")
            //    .Resolve(context => context.Source.IsFirst());

            //graphType.Field<NonNullGraphType<BooleanGraphType>>()
            //    .Name("_isLast")
            //    .Description("Is the content last in the list.")
            //    .Resolve(context => context.Source.IsLast());

            graphType.Field<NonNullGraphType<BooleanGraphType>>()
                .Name("_isVisible")
                .Description("Is the content visible.")
                .Resolve(context => context.Source.IsVisible());

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_name")
                .Description("Name of the content.")
                .Resolve(context => context.Source.Name);

            graphType.Field<PublishedContentInterfaceGraphType>()
                .Name("_parent")
                .Description("Parent of the content.")
                .Resolve(context => context.Source.Parent);

            graphType.Field<NonNullGraphType<IntGraphType>>()
                .Name("_sortOrder")
                .Description("SortOrder of the content.")
                .Resolve(context => context.Source.SortOrder);

            graphType.Field<NonNullGraphType<DateGraphType>>()
                .Name("_updateDate")
                .Description("Update date of the content.")
                .Resolve(context => context.Source.UpdateDate);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_url")
                .Description("Url of the content.")
                .Resolve(context => context.Source.Url);

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_urlAbsolute")
                .Description("Absolute url of the content.")
                .Resolve(context => context.Source.Url(mode: UrlMode.Absolute));

            graphType.Field<NonNullGraphType<StringGraphType>>()
                .Name("_writerName")
                .Description("Name of the content writer.")
                .Resolve(context => context.Source.WriterName);

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
    }
}
