using System;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Reflection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedPropertyFieldType : FieldType
    {
        public PublishedPropertyFieldType(IPublishedContentType contentType, IPropertyType propertyType,
            ITypeRegistry typeRegistry, IUmbracoContextFactory umbracoContextFactory, IPublishedRouter publishedRouter, IHttpContextAccessor httpContextAccessor)
        {
            var publishedPropertyType = contentType.GetPropertyType(propertyType.Alias);

            var type = publishedPropertyType.ClrType.GetTypeInfo();
            var unwrappedTypeInfo = type.Unwrap();

            if (typeof(IPublishedContent).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedContent).GetTypeInfo();
            else if (typeof(IPublishedElement).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedElement).GetTypeInfo();

            var propertyGraphType = typeRegistry.Get(unwrappedTypeInfo) ?? typeof(StringGraphType).GetTypeInfo();
            // The Grid data type declares its return type as a JToken, but is actually a JObject.  The result is that without this check,
            // it is cast as an IEnumerable<JProperty> which causes problems when trying to serialize the graph to send to the client
            propertyGraphType = propertyGraphType.Wrap(type, propertyType.Mandatory, false, propertyType.PropertyEditorAlias != global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.Grid);

            if (propertyType.VariesByCulture())
            {
                Arguments = new QueryArguments(new QueryArgument(typeof(StringGraphType))
                {
                    Name = "culture"
                });
            }

            Type = propertyGraphType;
            Name = publishedPropertyType.Alias.ToCamelCase();
            Description = propertyType.Description;
            Resolver = new AsyncFieldResolver<IPublishedElement, object>(async context =>
            {
                object ResolveProperty() => context.Source.Value(propertyType.Alias, context.GetArgument<string>("culture"), fallback: Fallback.ToLanguage);
                if (umbracoContextFactory == null || publishedRouter == null) return ResolveProperty();

                var pc = context.Source as IPublishedContent;
                var url = pc?.Url();
                if (string.IsNullOrEmpty(url)) return ResolveProperty();

                using (var ucRef = umbracoContextFactory.EnsureUmbracoContext())
                {
                    var uc = ucRef.UmbracoContext;

                    if (uc.PublishedRequest?.PublishedContent?.Id != pc.Id)
                    {
                        var currentUrl = new Uri(httpContextAccessor.HttpContext.Request.GetEncodedUrl());
                        var request = await publishedRouter.CreateRequestAsync(new Uri(currentUrl, url));
                        uc.PublishedRequest = await publishedRouter.RouteRequestAsync(request, default);
                    }

                    return ResolveProperty();
                }
            });
        }
    }
}
