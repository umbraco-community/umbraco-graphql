using GraphQL;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedContentCompositionGraphType : PublishedContentInterfaceGraphType
    {
        public PublishedContentCompositionGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry)
        {
            Name = contentType.Alias.ToPascalCase() + "Published" + contentType.GetItemType();

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(publishedContentType, propertyType, typeRegistry));
        }
    }
}
