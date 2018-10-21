using GraphQL;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentCompositionGraphType : PublishedContentInterfaceGraphType
    {
        public PublishedContentCompositionGraphType(IContentTypeComposition contentType, PublishedItemType itemType)
        {
            Name = contentType.Alias.ToPascalCase();
            Description = contentType.Description;
            Metadata = new Dictionary<string, object>
            {
                ["contentTypeAlias"] = contentType.Alias,
            };

            this.AddUmbracoContentPropeties(contentType, itemType);
        }
    }
}
