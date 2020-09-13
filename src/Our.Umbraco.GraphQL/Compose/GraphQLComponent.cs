using System;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLComponent : IComponent
    {
        private readonly ITypeRegistry _typeRegistry;

        public GraphQLComponent(ITypeRegistry typeRegistry)
        {
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        }

        public void Initialize()
        {
            _typeRegistry.Add<IPublishedContentType, PublishedContentTypeGraphType>();
            _typeRegistry.Add<IPublishedElement, PublishedElementInterfaceGraphType>();
            _typeRegistry.Add<IPublishedContent, PublishedContentInterfaceGraphType>();
            _typeRegistry.Add<PublishedItemType, PublishedItemTypeGraphType>();
            _typeRegistry.Add<UrlMode, UrlModeGraphType>();
            _typeRegistry.Add<Udi, UdiGraphType>();
            _typeRegistry.Add<Link, LinkGraphType>();
            _typeRegistry.Add<LinkType, LinkGraphType>();

            _typeRegistry.Extend<Query, ExtendQueryWithUmbracoQuery>();
            _typeRegistry.Extend<UmbracoQuery, ExtendUmbracoQueryWithPublishedContentQuery>();
        }

        public void Terminate()
        {
        }
    }
}
