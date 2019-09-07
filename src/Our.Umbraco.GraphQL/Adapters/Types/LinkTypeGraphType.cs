using GraphQL.Types;
using Umbraco.Web.Models;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class LinkTypeGraphType : EnumerationGraphType<LinkType>
    {
        public LinkTypeGraphType()
        {
            Name = "LinkType";
        }
    }
}
