using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata("PublishedContentType")]
    public class PublishedContentTypeGraphType : ObjectGraphType<IPublishedContentType>
    {
        public PublishedContentTypeGraphType()
        {
            Name = "PublishedContentType";

            Field<NonNullGraphType<StringGraphType>>().Name("alias").Resolve(ctx => ctx.Source.Alias);
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>().Name("compositionAliases")
                .Resolve(ctx => ctx.Source.CompositionAliases);
            Field<NonNullGraphType<PublishedItemTypeGraphType>>().Name("itemType").Resolve(ctx => ctx.Source.ItemType);
            Field<NonNullGraphType<ContentVariationGraphType>>().Name("variations")
                .Resolve(ctx => ctx.Source.Variations);
        }
    }
}
