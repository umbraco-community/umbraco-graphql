using GraphQL.Types;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentInterfaceGraphType : InterfaceGraphType<IPublishedContent>
    {
        public PublishedContentInterfaceGraphType()
        {
            Name = "PublishedContent";

            // TODO: set this field name as a reserved property alias
            Field<NonNullGraphType<PublishedContentDataGraphType>>()
                .Name("_contentData")
                .Description("Built in published content data.")
                .Resolve(context => context.Source)
                .SetDoctypeMetadata(GetMetadata<string>("documentTypeAlias"));
        }
    }
}
