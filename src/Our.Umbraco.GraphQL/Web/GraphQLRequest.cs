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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Cors;
using Umbraco.Web;

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