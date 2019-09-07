using GraphQL;
using GraphQL.Conversion;
using GraphQL.Http;
using GraphQL.Utilities;
using LightInject;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Owin;
using StackExchange.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Cors;
using Umbraco.Web;

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
