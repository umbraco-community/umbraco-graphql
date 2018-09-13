using System.Collections.Generic;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class UmbracoQuery : ObjectGraphType
    {
        public UmbracoQuery(IEnumerable<IGraphType> documentTypes)
        {
            Field<NonNullGraphType<PublishedContentQuery>>()
                .Name("content")
                .Resolve(context => context.ReturnType)
                .Type(new NonNullGraphType(new PublishedContentQuery(documentTypes)));
        }
    }
}
