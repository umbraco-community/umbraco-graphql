using System;
using Examine;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Examine.Types;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;

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
            _typeRegistry.Add<LinkType, LinkTypeGraphType>();
            _typeRegistry.Add<ISearchResults, SearchResultsInterfaceGraphType>();
            _typeRegistry.Add<ISearchResult, SearchResultInterfaceGraphType>();

            _typeRegistry.Extend<Query, ExtendQueryWithUmbracoQuery>();
            _typeRegistry.Extend<UmbracoQuery, ExtendUmbracoQueryWithPublishedContentQuery>();
            _typeRegistry.Extend<UmbracoQuery, ExtendUmbracoQueryWithExamineQuery>();
        }

        public void Terminate()
        {
        }
    }
}
