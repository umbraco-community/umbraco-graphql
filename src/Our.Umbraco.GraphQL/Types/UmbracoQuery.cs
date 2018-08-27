using GraphQL.Types;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{

    public class UmbracoQuery : ObjectGraphType
    {
        public UmbracoQuery()
        {
            Field<PublishedContentGraphType>()
                .Name("content")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique content id")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    var id = context.GetArgument<int>("id");
                    return userContext.Umbraco.TypedContent(id);
                });

            Field<ListGraphType<PublishedContentGraphType>>()
                .Name("contentAtRoot")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    return userContext.Umbraco.TypedContentAtRoot();
                });
        }
    }
}
