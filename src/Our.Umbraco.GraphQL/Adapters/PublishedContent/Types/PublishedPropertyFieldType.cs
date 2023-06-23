using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Reflection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedPropertyFieldType : FieldType
    {
        public PublishedPropertyFieldType(IPublishedContentType contentType, IPropertyType propertyType,
            ITypeRegistry typeRegistry, IUmbracoContextFactory umbracoContextFactory, IPublishedRouter publishedRouter, IHttpContextAccessor httpContextAccessor,
            ILogger<PublishedPropertyFieldType> logger)
        {
            var publishedPropertyType = contentType.GetPropertyType(propertyType.Alias);

            var type = publishedPropertyType.ClrType.GetTypeInfo();
            var unwrappedTypeInfo = type.Unwrap();

            if (typeof(IPublishedContent).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedContent).GetTypeInfo();
            else if (typeof(IPublishedElement).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedElement).GetTypeInfo();
            else if (typeof(BlockListItem).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(BlockListItem).GetTypeInfo();

            var mappedType = typeRegistry.Get(unwrappedTypeInfo);
            if (mappedType == null)
            {
                logger.LogWarning($"Missing property type for ${unwrappedTypeInfo.FullName}");
            }

            var propertyGraphType = mappedType ?? typeof(StringGraphType).GetTypeInfo();
            // The Grid data type declares its return type as a JToken, but is actually a JObject.  The result is that without this check,
            // it is cast as an IEnumerable<JProperty> which causes problems when trying to serialize the graph to send to the client
            propertyGraphType = propertyGraphType.Wrap(type, propertyType.Mandatory, false, propertyType.PropertyEditorAlias != global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.Grid, out var isEnumerable);

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
                object ResolveProperty() => NormalizeResult(mappedType, isEnumerable, context.Source.Value(propertyType.Alias, context.GetArgument<string>("culture"), fallback: Fallback.ToLanguage));
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

        private object NormalizeResult(TypeInfo mappedType, bool isEnumerable, object result)
        {
            if (mappedType != null) return result;
            if (result == null) return "null";

            if (isEnumerable && result is IEnumerable en) return en.Cast<object>().Select(JsonString);
            else return JsonString(result);
        }

        private string JsonString(object value) => value != null ? JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) : "null";
    }
}
