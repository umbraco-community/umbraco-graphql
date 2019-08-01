using GraphQL.Types;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentQueryGraphType : ObjectGraphType
    {
        public PublishedContentQueryGraphType(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentQuery";

            Field<PublishedContentInterfaceGraphType>()
                .Name("byId")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique content id")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    var id = context.GetArgument<int>("id");
                    return userContext.UmbracoContext.Content.GetById(id);
                });

            Field<NonNullGraphType<PublishedContentByTypeQueryGraphType>>()
                .Type(new NonNullGraphType(new PublishedContentByTypeQueryGraphType(documentGraphTypes)))
                .Name("byType")
                .Resolve(context => context.ReturnType);

            Field<NonNullGraphType<PublishedContentAtRootQueryGraphType>>()
                .Type(new NonNullGraphType(new PublishedContentAtRootQueryGraphType(documentGraphTypes)))
                .Name("atRoot")
                .Resolve(context => context.ReturnType);
        }
    }
}
