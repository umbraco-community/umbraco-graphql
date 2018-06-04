using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLRequest : List<GraphQLQuery>
    {
        public GraphQLRequest(IEnumerable<GraphQLQuery> list, bool isBatched) : base(list)
        {
            IsBatched = isBatched;
        }

        public bool IsBatched { get; }
    }
}