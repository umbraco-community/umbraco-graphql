using System;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL
{
    public class UmbracoGraphQLContext
    {
        public UmbracoGraphQLContext(Uri requestUri,  UmbracoContext umbracoContext, string culture)
        {
            RequestUri = requestUri;
            UmbracoContext = umbracoContext ?? throw new ArgumentNullException(nameof(umbracoContext));
            Culture = culture;
        }

        public UmbracoContext UmbracoContext { get; }
        public Uri RequestUri { get; }
        public string Culture { get; }
    }
}