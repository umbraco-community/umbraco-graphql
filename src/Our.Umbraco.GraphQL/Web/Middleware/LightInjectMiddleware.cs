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

namespace Our.Umbraco.GraphQL.Web.Middleware
{

    internal class LightInjectMiddleware : OwinMiddleware
    {
        private readonly IServiceContainer container;

        public LightInjectMiddleware(OwinMiddleware next, IServiceContainer container)
            : base(next)
        {
            this.container = container;
        }

        public async override Task Invoke(IOwinContext context)
        {
            using (Scope scope = container.BeginScope())
            {
                context.Set("lightinject:scope", scope);
                await Next.Invoke(context);
            }
        }
    }
}