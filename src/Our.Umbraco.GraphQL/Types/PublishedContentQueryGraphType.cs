using System;
using System.Collections.Generic;
using GraphQL.Types;
using Umbraco.Core.Configuration;
using Umbraco.Web.Routing;

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
                    return userContext.Umbraco.TypedContent(id);
                });

            Field<PublishedContentInterfaceGraphType>()
                .Name("byUrl")
                .Argument<NonNullGraphType<StringGraphType>>("url", "The relative content url")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
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

            Field<NonNullGraphType<PublishedContentAtRootQueryGraphType>>()
                .Name("byType")
                .Resolve(context => context.ReturnType)
                .Type(new NonNullGraphType(new PublishedContentByTypeQueryGraphType(documentGraphTypes)));

            Field<NonNullGraphType<PublishedContentAtRootQueryGraphType>>()
                .Name("atRoot")
                .Resolve(context => context.ReturnType)
                .Type(new NonNullGraphType(new PublishedContentAtRootQueryGraphType(documentGraphTypes)));
        }
    }
}
