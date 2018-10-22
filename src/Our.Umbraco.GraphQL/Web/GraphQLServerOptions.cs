using System;
using System.Web.Cors;
using Microsoft.Owin.Cors;
using Umbraco.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace Our.Umbraco.GraphQL.Web
{
    public class GraphQLServerOptions
    {
        public GraphQLServerOptions() : this((ICorsPolicyProvider)null)
        {
        }

        public GraphQLServerOptions(CorsPolicy corsPolicy) : this(
            new CorsPolicyProvider
            {
                PolicyResolver = _ => Task.FromResult(corsPolicy)
            }
        )
        {
        }

        public GraphQLServerOptions(ICorsPolicyProvider corsPolicyProvder)
        {
            CorsPolicyProvder = corsPolicyProvder ?? new CorsPolicyProvider
            {
                PolicyResolver = _ => Task.FromResult(
                    new CorsPolicy
                    {
                        Headers =
                        {
                            "X-Requested-With",
                            "X-HTTP-Method-Override",
                            "Content-Type",
                            "Cache-Control",
                            "Accept"
                        },
                        AllowAnyOrigin = true,
                        Methods = { "POST" }
                    }
                )
            };
        }

        public ICorsPolicyProvider CorsPolicyProvder { get; }
        //public ComplexityConfiguration ComplexityConfiguration { get; set; }
        public bool Debug { get; set; }
        //public bool ExposeGraphiQL { get; set; } = true;
        //public bool EnableLogin { get; set; } = false;
        public bool EnableMetrics { get; set; } = false;
        //public bool ExposeSchema { get; set; } = true;
    }
}
