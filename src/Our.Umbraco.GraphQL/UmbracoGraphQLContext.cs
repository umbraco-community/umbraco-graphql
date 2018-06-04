using System;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL
{
    public class UmbracoGraphQLContext
    {
        private UmbracoHelper _umbraco;

        public UmbracoGraphQLContext(Uri requestUri, ApplicationContext applicationContext, UmbracoContext umbracoContext, GraphQLServerOptions options)
        {
            RequestUri = requestUri;
            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            UmbracoContext = umbracoContext ?? throw new ArgumentNullException(nameof(umbracoContext));
        }

        public ApplicationContext ApplicationContext { get;  }
        public GraphQLServerOptions Options { get; }
        public Uri RequestUri { get; }
        public UmbracoHelper Umbraco => _umbraco ?? (_umbraco = new UmbracoHelper(UmbracoContext));
        public UmbracoContext UmbracoContext { get; }
    }
}
