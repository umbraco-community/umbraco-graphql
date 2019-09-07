using GraphQL.Types;
using Umbraco.Web.Models;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class LinkGraphType : ObjectGraphType<Link>
    {
        public LinkGraphType()
        {
            Name = "Link";

            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("target");
            Field<NonNullGraphType<LinkTypeGraphType>>("type");
            Field<NonNullGraphType<StringGraphType>>("url");
            Field<UdiGraphType>("udi");
        }
    }
}
