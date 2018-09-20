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

            Field<PublishedContentGraphType>()
                .Name("byUrl")
                .Argument<NonNullGraphType<StringGraphType>>("url", "The relative content url")
                .Resolve(context =>
                {
                    var userContext = context.UmbracoUserContext();
                    var url = context.GetArgument<string>("url");
                    
                    var pcr = new PublishedContentRequest(
                        new Uri(userContext.RequestUri, url),
                        userContext.UmbracoContext.RoutingContext,
                        UmbracoConfig.For.UmbracoSettings().WebRouting,
                        null
                    );

                    pcr.Prepare();

                    if (pcr.IsRedirect || pcr.IsRedirectPermanent || pcr.Is404)
                    {
                        return null;
                    }
                    
                    return pcr.PublishedContent;
                });

            Field<NonNullGraphType<PublishedContentAtRootQuery>>()
                .Name("atRoot")
                .Resolve(context => context.ReturnType)
                .Type(new NonNullGraphType(new PublishedContentAtRootQuery(documentGraphTypes)));
        }
    }
}
