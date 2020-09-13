using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class PublishedPropertyFieldType : FieldType
    {
        public PublishedPropertyFieldType(IPublishedContentType contentType, PropertyType propertyType,
            ITypeRegistry typeRegistry)
        {
            var publishedPropertyType = contentType.GetPropertyType(propertyType.Alias);

            var type = publishedPropertyType.ClrType.GetTypeInfo();
            var unwrappedTypeInfo = type.Unwrap();

            if (typeof(IPublishedContent).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedContent).GetTypeInfo();
            else if (typeof(IPublishedElement).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedElement).GetTypeInfo();

            var propertyGraphType = typeRegistry.Get(unwrappedTypeInfo) ?? typeof(StringGraphType).GetTypeInfo();

            propertyGraphType = propertyGraphType.Wrap(type, propertyType.Mandatory, false);

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
            Resolver = new FuncFieldResolver<IPublishedElement, object>(context =>
                context.Source.Value(propertyType.Alias, context.GetArgument<string>("culture"),
                    fallback: Fallback.ToLanguage));
        }
    }
}
