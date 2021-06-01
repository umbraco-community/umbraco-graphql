using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Builders;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    internal static class PublishedGraphTypeExtensions
    {
        public static void AddBuiltInFields(this ComplexGraphType<IPublishedElement> graphType)
        {
            graphType.Field<NonNullGraphType<PublishedContentTypeGraphType>>().Name("_contentType")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedElement x) => x.ContentType))
                .Resolve(ctx => ctx.Source.ContentType);

            graphType.Field<NonNullGraphType<IdGraphType>>().Name("_id")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedElement x) => x.Key))
                .Resolve(ctx => new Id(ctx.Source.Key.ToString()));
        }

        public static void AddBuiltInFields(this ComplexGraphType<IPublishedContent> graphType)
        {
            graphType.Connection<PublishedContentInterfaceGraphType>().Name("_ancestors")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.Ancestors()))
                .Bidirectional()
                .Orderable<IPublishedContent, PublishedContentInterfaceGraphType>()
                .Resolve(ctx =>
                    ctx.Source.Ancestors()
                        .OrderBy(ctx.GetArgument<IList<OrderBy>>("orderBy"))
                        .ToConnection(x => x.Key, ctx.First, ctx.After, ctx.Last, ctx.Before));

            graphType.Connection<PublishedContentInterfaceGraphType>().Name("_children")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.Children(null)))
                .Bidirectional()
                .Orderable<IPublishedContent, PublishedContentInterfaceGraphType>()
                .Resolve(ctx =>
                    ctx.Source.Children(ctx.GetArgument<string>("culture"))
                        .OrderBy(ctx.GetArgument<IList<OrderBy>>("orderBy"))
                        .ToConnection(x => x.Key, ctx.First, ctx.After, ctx.Last, ctx.Before));

            graphType.Field<NonNullGraphType<PublishedContentTypeGraphType>>()
                .Metadata(nameof(MemberInfo), GetMember((IPublishedElement x) => x.ContentType))
                .Name("_contentType")
                .Resolve(ctx => ctx.Source.ContentType);

            graphType.Field<NonNullGraphType<DateTimeGraphType>>().Name("_createDate")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.CreateDate))
                .Resolve(x => x.Source.CreateDate);

            graphType.Field<NonNullGraphType<StringGraphType>>().Name("_creatorName")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.CreatorName()))
                .Resolve(ctx => ctx.Source.CreatorName());

            graphType.Field<NonNullGraphType<IdGraphType>>().Name("_id")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedElement x) => x.Key))
                .Resolve(ctx => new Id(ctx.Source.Key.ToString()));

            graphType.Field<NonNullGraphType<IntGraphType>>().Name("_level")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.Level))
                .Resolve(ctx => ctx.Source.Level);

            graphType.Field<StringGraphType>().Name("_name")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.Name(null)))
                .Argument<StringGraphType>("culture", "The culture.")
                .Resolve(ctx => ctx.Source.Name(ctx.GetArgument<string>("culture")));

            graphType.Field<PublishedContentInterfaceGraphType>().Name("_parent")
                .Resolve(ctx => ctx.Source.Parent);

            graphType.Field<NonNullGraphType<IntGraphType>>().Name("_sortOrder")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.SortOrder))
                .Resolve(ctx => ctx.Source.SortOrder);

            graphType.Field<StringGraphType>().Name("_template")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.GetTemplateAlias()))
                .Resolve(ctx => ctx.Source.GetTemplateAlias());

            graphType.Field<StringGraphType>().Name("_url")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.Url(null, UrlMode.Default)))
                .Argument<StringGraphType>("culture", "The culture.")
                .Argument<UrlModeGraphType>("mode", "The url mode.")
                .Resolve(ctx =>
                    ctx.Source.Url(ctx.GetArgument<string>("culture"), ctx.GetArgument("mode", UrlMode.Default)));

            graphType.Field<DateTimeGraphType>().Name("_updateDate")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.CultureDate(null)))
                .Argument<StringGraphType>("culture", "The culture.")
                .Resolve(ctx => ctx.Source.CultureDate(ctx.GetArgument<string>("culture")));

            graphType.Field<NonNullGraphType<StringGraphType>>().Name("_writerName")
                .Metadata(nameof(MemberInfo), GetMember((IPublishedContent x) => x.WriterName()))
                .Resolve(ctx => ctx.Source.WriterName());
        }

        private static MemberInfo GetMember<TSource, TReturn>(Expression<Func<TSource, TReturn>> expression)
        {
            switch (expression.Body)
            {
                case MethodCallExpression methodCallExpression:
                    return methodCallExpression.Method;
                case MemberExpression memberExpression:
                    return memberExpression.Member;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }
    }
}
