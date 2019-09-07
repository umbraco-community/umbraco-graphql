using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedElementGraphType : ObjectGraphType<IPublishedElement>
    {
        public PublishedElementGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry)
        {
            Name = contentType.Alias.ToPascalCase() + "PublishedElement";
            IsTypeOf = x => ((IPublishedElement) x).ContentType.Alias == contentType.Alias;

            Interface<PublishedElementInterfaceGraphType>();

            Field<NonNullGraphType<PublishedContentTypeGraphType>>()
                .Name("_contentType")
                .Resolve(ctx => ctx.Source.ContentType);

            Field<NonNullGraphType<IdGraphType>>()
                .Name("_id")
                .Resolve(ctx => new Id(ctx.Source.Key.ToString()));

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(publishedContentType, propertyType, typeRegistry));
        }
    }
}
