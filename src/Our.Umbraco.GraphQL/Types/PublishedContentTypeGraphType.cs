using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentTypeGraphType : ObjectGraphType<PublishedContentType>
    {
        public PublishedContentTypeGraphType()
        {
            Name = "PublishedContentType";

            Field<NonNullGraphType<StringGraphType>>("alias", "The alias of the content type.");
            Field<NonNullGraphType<IdGraphType>>("id", "The id of the content type.");
        }
    }
}
