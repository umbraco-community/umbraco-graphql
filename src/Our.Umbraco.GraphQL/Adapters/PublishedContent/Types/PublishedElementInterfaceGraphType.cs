using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata("PublishedElement")]
    public class PublishedElementInterfaceGraphType : InterfaceGraphType<IPublishedElement>
    {
        public PublishedElementInterfaceGraphType()
        {
            Name = "PublishedElement";

            Field<NonNullGraphType<PublishedContentTypeGraphType>>().Name("_contentType");
            Field<NonNullGraphType<IdGraphType>>().Name("_id");
        }
    }
}
