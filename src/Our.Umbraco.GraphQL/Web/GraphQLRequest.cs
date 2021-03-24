using System.Collections;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Web
{

    public class GraphQLRequest : IEnumerable<GraphQLQuery>
    {
        private IEnumerable<GraphQLQuery> _queries;

        public GraphQLRequest(GraphQLQuery query)
        {
            _queries = new[] {query};
            IsBatched = false;
        }
        public GraphQLRequest(IEnumerable<GraphQLQuery> queries)
        {
            _queries = queries;

            IsBatched = true;
        }

        public bool IsBatched { get; }

        public IEnumerator<GraphQLQuery> GetEnumerator() => _queries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
