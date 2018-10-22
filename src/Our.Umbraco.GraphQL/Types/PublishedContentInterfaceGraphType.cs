using GraphQL;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    [GraphQLMetadata("PublishedContent")]
    public class PublishedContentInterfaceGraphType : InterfaceGraphType<IPublishedContent>
    {
        public PublishedContentInterfaceGraphType()
        {
            Name = "PublishedContent";

            Field<NonNullGraphType<PublishedContentDataGraphType>>()
                .Name("_contentData")
                .Description("Built in published content data.")
                .Resolve(context => context.Source);
        }
    }
}
