using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class PublishedContentInterfaceGraphType : InterfaceGraphType<IPublishedContent>
    {
        private const string TypeName = "PublishedContent";
        public PublishedContentInterfaceGraphType()
        {
            Name = TypeName;

            this.AddBuiltInFields();
        }
    }
}
