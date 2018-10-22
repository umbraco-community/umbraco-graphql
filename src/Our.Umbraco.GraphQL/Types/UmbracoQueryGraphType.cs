using GraphQL.Types;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Types
{
    public class UmbracoQueryGraphType : ObjectGraphType
    {
        public UmbracoQueryGraphType(IEnumerable<IGraphType> contentGraphTypes, IEnumerable<IGraphType> mediaGraphTypes)
        {
            Name = "UmbracoQuery";

            Field<NonNullGraphType<PublishedContentQueryGraphType>>()
                .Type(new NonNullGraphType(new PublishedContentQueryGraphType(contentGraphTypes)))
                .Name("content")
                .Resolve(context => context.ReturnType);
        }
    }
}
