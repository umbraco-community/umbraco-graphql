using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata("ContentVariation")]
    public class ContentVariationGraphType : EnumerationGraphType<ContentVariation>
    {
    }
}
