using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class PublishedItemTypeGraphType : EnumerationGraphType<PublishedItemType>
    {
        private const string TypeName = "PublishedItemType";

        public PublishedItemTypeGraphType()
        {
            Name = TypeName;
        }
    }
}
