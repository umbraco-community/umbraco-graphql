using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedElementGraphType : ObjectGraphType<IPublishedElement>
    {
        public PublishedElementGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry, IHttpContextAccessor httpContextAccessor)
        {
            Name = $"{contentType.Alias.ToPascalCase()}PublishedElement";
            IsTypeOf = x => ((IPublishedElement) x).ContentType.Alias == contentType.Alias;

            Interface<PublishedElementInterfaceGraphType>();

            this.AddBuiltInFields();

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(publishedContentType, propertyType, typeRegistry, null, null, httpContextAccessor));
        }
    }
}
