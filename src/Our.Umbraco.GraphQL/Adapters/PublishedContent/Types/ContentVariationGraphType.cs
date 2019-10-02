using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class ContentVariationGraphType : EnumerationGraphType<ContentVariation>
    {
        private const string TypeName = "ContentVariation";

        public ContentVariationGraphType()
        {
            Name = TypeName;
            Description = "Indicates how values can vary.";
        }
    }
}
