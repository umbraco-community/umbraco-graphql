using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentAtRootQueryGraphType : ObjectGraphType
    {
        public PublishedContentAtRootQueryGraphType(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentAtRootQuery";

            Field<NonNullGraphType<ListGraphType<PublishedContentInterfaceGraphType>>>()
                .Name("all")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    return userContext.UmbracoContext.ContentCache.GetAtRoot();
                });

            foreach (var type in documentGraphTypes.Where(x => x.GetMetadata<bool>(Constants.Metadata.AllowedAtRoot)))
            {
                var contentTypeAlias = type.GetMetadata<string>(Constants.Metadata.ContentTypeAlias);

                Field<NonNullGraphType<ListGraphType<PublishedContentInterfaceGraphType>>>()
                    .Name(contentTypeAlias.ToPascalCase())
                    .Type(new NonNullGraphType(new ListGraphType(type)))
                    .Resolve(context =>
                    {
                        var userContext = (UmbracoGraphQLContext)context.UserContext;
                        return userContext.UmbracoContext.ContentCache.GetByXPath($"/root/{contentTypeAlias}");
                    });
            }
        }
    }
}
