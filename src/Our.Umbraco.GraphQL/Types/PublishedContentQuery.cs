using System.Collections.Generic;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentQuery : ObjectGraphType
    {
        public PublishedContentQuery(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentQuery";

            Field<PublishedContentGraphType>()
                .Name("byId")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique content id")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    var id = context.GetArgument<int>("id");
                    return userContext.Umbraco.TypedContent(id);
                });

            Field<NonNullGraphType<PublishedContentAtRootQuery>>()
                .Name("atRoot")
                .Resolve(context => context.ReturnType)
                .Type(new NonNullGraphType(new PublishedContentAtRootQuery(documentGraphTypes)));
        }
    }
}
