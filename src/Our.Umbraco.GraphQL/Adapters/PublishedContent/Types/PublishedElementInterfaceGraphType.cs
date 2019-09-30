using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata(TypeName)]
    public class PublishedElementInterfaceGraphType : InterfaceGraphType<IPublishedElement>
    {
        private const string TypeName = "PublishedElement";
        public PublishedElementInterfaceGraphType()
        {
            Name = TypeName;

            this.AddBuiltInFields();
        }
    }
}
