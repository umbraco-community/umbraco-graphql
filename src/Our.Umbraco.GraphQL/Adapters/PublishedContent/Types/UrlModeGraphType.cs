using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class UrlModeGraphType : EnumerationGraphType<UrlMode>
    {
        private const string TypeName = "UrlMode";

        public UrlModeGraphType()
        {
            Name = TypeName;
        }
    }
}
