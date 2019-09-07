using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    [GraphQLMetadata("PublishedContent")]
    public class PublishedContentInterfaceGraphType : InterfaceGraphType<IPublishedContent>
    {
        public PublishedContentInterfaceGraphType()
        {
            Name = "PublishedContent";

            Connection<PublishedContentInterfaceGraphType>().Name("_ancestors").Bidirectional();
            Connection<PublishedContentInterfaceGraphType>().Name("_children").Bidirectional();
            Field<NonNullGraphType<DateTimeGraphType>>().Name("_createDate");
            Field<NonNullGraphType<StringGraphType>>().Name("_creatorName");
            Field<NonNullGraphType<PublishedContentTypeGraphType>>().Name("_contentType");
            Field<NonNullGraphType<IdGraphType>>().Name("_id");
            Field<NonNullGraphType<IntGraphType>>().Name("_level");
            Field<StringGraphType>().Name("_name")
                .Argument<StringGraphType>("culture", "The culture.");
            Field<PublishedContentInterfaceGraphType>().Name("_parent");
            Field<NonNullGraphType<IntGraphType>>().Name("_sortOrder");
            Field<StringGraphType>().Name("_url")
                .Argument<StringGraphType>("culture", "The culture.")
                .Argument<UrlModeGraphType>("mode", "The url mode.");
            Field<DateTimeGraphType>().Name("_updateDate")
                .Argument<StringGraphType>("culture", "The culture.");
            Field<NonNullGraphType<StringGraphType>>().Name("_writerName");
        }
    }
}
