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

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }

        [JsonConverter(typeof(InputConverter))]
        public Inputs Variables { get; set; }
    }
}