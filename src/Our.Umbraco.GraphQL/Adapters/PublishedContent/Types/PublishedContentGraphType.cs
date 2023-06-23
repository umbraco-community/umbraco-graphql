using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedContentGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentGraphType(IContentTypeComposition contentType,
            IPublishedContentType publishedContentType, ITypeRegistry typeRegistry,
            IUmbracoContextFactory umbracoContextFactory, IPublishedRouter publishedRouter,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory)
        {
            Name = $"{contentType.Alias.ToPascalCase()}Published{contentType.GetItemType()}";
            IsTypeOf = x => ((IPublishedContent)x).ContentType.Alias == contentType.Alias;

            Interface<PublishedContentInterfaceGraphType>();

            this.AddBuiltInFields();

            foreach (var propertyType in contentType.CompositionPropertyTypes)
                base.AddField(new PublishedPropertyFieldType(
                    publishedContentType,
                    propertyType,
                    typeRegistry,
                    umbracoContextFactory,
                    publishedRouter,
                    httpContextAccessor,
                    loggerFactory.CreateLogger<PublishedPropertyFieldType>()
                    ));
        }
    }
}
