using GraphQL;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentCompositionGraphType : PublishedContentInterfaceGraphType
    {
        public PublishedContentCompositionGraphType(IContentTypeComposition contentType, IPublishedContentGraphTypeBuilder graphTypeBuilder)
        {
            Name = contentType.Alias.ToPascalCase();
            Description = contentType.Description;
            Metadata = new Dictionary<string, object>
            {
                [Constants.Metadata.ContentTypeAlias] = contentType.Alias,
            };

            foreach(var field in graphTypeBuilder.BuildContentPropertyFieldTypes(contentType))
            {
                AddField(field)/*.SetPermissions(this, true)*/;
            }
        }
    }
}
