using System.Collections.Generic;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentByTypeQueryGraphType : ObjectGraphType
    {
        public PublishedContentByTypeQueryGraphType(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentByTypeQuery";

            foreach (var type in documentGraphTypes)
            {
                string documentTypeAlias = type.GetMetadata<string>(Constants.Metadata.ContentTypeAlias);

                Field<PublishedContentInterfaceGraphType>()
                    .Name(type.Name)
                    .Argument<NonNullGraphType<IdGraphType>>("id", "The unique content id")
                    .Type(type)
                    .Resolve(context =>
                    {
                        var userContext = (UmbracoGraphQLContext)context.UserContext;
                        var id = context.GetArgument<int>("id");
                        return userContext.Umbraco.TypedContent(id);
                    });
            }
        }
    }
}
