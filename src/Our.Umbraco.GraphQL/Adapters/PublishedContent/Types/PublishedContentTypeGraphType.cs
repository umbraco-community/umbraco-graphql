using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class PublishedContentTypeGraphType : ObjectGraphType<IPublishedContentType>
    {
        private const string TypeName = "PublishedContentType";
        public PublishedContentTypeGraphType()
        {
            Name = TypeName;

            Field<NonNullGraphType<StringGraphType>>().Name("alias").Resolve(ctx => ctx.Source.Alias);
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>().Name("compositionAliases")
                .Resolve(ctx => ctx.Source.CompositionAliases);
            Field<NonNullGraphType<PublishedItemTypeGraphType>>().Name("itemType").Resolve(ctx => ctx.Source.ItemType);
            Field<NonNullGraphType<ContentVariationGraphType>>().Name("variations")
                .Resolve(ctx => ctx.Source.Variations);
        }
    }
}
