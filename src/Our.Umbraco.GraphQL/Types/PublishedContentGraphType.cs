using GraphQL;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentGraphType(IContentTypeComposition contentType, IDictionary<string, IInterfaceGraphType> interfaceGraphTypes, IPublishedContentGraphTypeBuilder graphTypeBuilder)
        {
            Name = contentType.Alias.ToPascalCase();
            Description = contentType.Description;
            IsTypeOf = content => ((IPublishedContent)content).ContentType.Alias == contentType.Alias;
            Metadata = new Dictionary<string, object>
            {
                [Constants.Metadata.ContentTypeAlias] = contentType.Alias,
                [Constants.Metadata.AllowedAtRoot] = contentType.AllowedAsRoot,
                [Constants.Metadata.AllowedChildren] = contentType.AllowedContentTypes.Select(x => x.Alias).ToArray(),
            };

            Interface<PublishedContentInterfaceGraphType>();

            foreach (IContentTypeComposition composition in contentType.ContentTypeComposition)
            {
                if (interfaceGraphTypes.TryGetValue(composition.Alias, out IInterfaceGraphType interfaceType))
                {
                    AddResolvedInterface(interfaceType);
                }
            }

            this.AddUmbracoBuiltInProperties();
            foreach (var field in graphTypeBuilder.BuildContentPropertyFieldTypes(contentType))
            {
                AddField(field);
            }
        }
    }
}
