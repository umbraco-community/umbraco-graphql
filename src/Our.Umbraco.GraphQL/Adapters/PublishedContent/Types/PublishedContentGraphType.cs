using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedContentGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry)
        {
            Name = contentType.Alias.ToPascalCase() + "Published" + contentType.GetItemType();
            IsTypeOf = x => ((IPublishedContent)x).ContentType.Alias == contentType.Alias;

            Interface<PublishedContentInterfaceGraphType>();

            Connection<PublishedContentInterfaceGraphType>()
                .Name("_ancestors")
                .Bidirectional()
                .Resolve(ctx =>
                    ctx.Source.Ancestors().ToConnection(x => x.Key, ctx.First, ctx.After, ctx.Last, ctx.Before));

            Connection<PublishedContentInterfaceGraphType>()
                .Name("_children")
                .Bidirectional()
                .Resolve(ctx =>
                    ctx.Source.Children(ctx.GetArgument<string>("culture"))
                        .ToConnection(x => x.Key, ctx.First, ctx.After, ctx.Last, ctx.Before));

            Field<NonNullGraphType<DateTimeGraphType>>().Name("_createDate")
                .Resolve(x => x.Source.CreateDate);
            Field<NonNullGraphType<StringGraphType>>().Name("_creatorName")
                .Resolve(ctx => ctx.Source.CreatorName);
            Field<NonNullGraphType<PublishedContentTypeGraphType>>().Name("_contentType")
                .Resolve(ctx => ctx.Source.ContentType);
            Field<NonNullGraphType<IdGraphType>>().Name("_id").Resolve(x => new Id(x.Source.Key.ToString()));
            Field<NonNullGraphType<IntGraphType>>().Name("_level").Resolve(ctx => ctx.Source.Level);

            Field<StringGraphType>()
                .Name("_name")
                .Argument<StringGraphType>("culture", "The culture.")
                .Resolve(ctx => ctx.Source.Name(ctx.GetArgument<string>("culture")));

            Field<PublishedContentInterfaceGraphType>().Name("_parent").Resolve(ctx => ctx.Source.Parent);
            Field<NonNullGraphType<IntGraphType>>().Name("_sortOrder").Resolve(ctx => ctx.Source.SortOrder);

            Field<StringGraphType>()
                .Name("_url")
                .Argument<StringGraphType>("culture", "The culture.")
                .Argument<UrlModeGraphType>("mode", "The url mode.")
                .Resolve(ctx =>
                    ctx.Source.Url(ctx.GetArgument<string>("culture"), ctx.GetArgument("mode", UrlMode.Default)));

            Field<DateTimeGraphType>()
                .Name("_updateDate")
                .Argument<StringGraphType>("culture", "The culture.")
                .Resolve(ctx => ctx.Source.CultureDate(ctx.GetArgument<string>("culture")));

            Field<NonNullGraphType<StringGraphType>>().Name("_writerName")
                .Resolve(ctx => ctx.Source.WriterName);

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(publishedContentType, propertyType, typeRegistry));
        }
    }
}
