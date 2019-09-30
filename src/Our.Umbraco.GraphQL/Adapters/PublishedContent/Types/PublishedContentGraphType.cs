using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedContentGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry)
        {
            Name = $"{contentType.Alias.ToPascalCase()}Published{contentType.GetItemType()}";
            IsTypeOf = x => ((IPublishedContent)x).ContentType.Alias == contentType.Alias;

            Interface<PublishedContentInterfaceGraphType>();

            this.AddBuiltInFields();

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(publishedContentType, propertyType, typeRegistry));
        }
    }
}
