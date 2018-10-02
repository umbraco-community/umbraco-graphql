using System.Collections.Generic;
using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentAtRootQuery : ObjectGraphType
    {
        public PublishedContentAtRootQuery(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentAtRootQuery";

            Field<NonNullGraphType<ListGraphType<PublishedContentGraphType>>>()
                .Name("all")
                .Resolve(context =>
                {
                    var userContext = (UmbracoGraphQLContext)context.UserContext;
                    return userContext.Umbraco.TypedContentAtRoot();
                });

            foreach (var type in documentGraphTypes.Where(x => x.GetMetadata<bool>("allowedAtRoot")))
            {
                string documentTypeAlias = type.GetMetadata<string>("documentTypeAlias");

                this.AddField(
                    new FieldType
                    {
                        Name = type.Name,
                        ResolvedType = new NonNullGraphType(new ListGraphType(type)),
                        Resolver = new FuncFieldResolver<object>(context =>
                        {
                            var userContext = (UmbracoGraphQLContext)context.UserContext;
                            return userContext.Umbraco.TypedContentAtXPath($"/root/{documentTypeAlias}");
                        })
                    }
                );
            }
        }
    }
}
