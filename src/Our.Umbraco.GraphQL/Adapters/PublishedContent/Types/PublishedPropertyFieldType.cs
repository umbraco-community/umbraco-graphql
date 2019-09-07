using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
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
            var unwrappedTypeInfo = UnwrapTypeInfo(type);

            if (typeof(IPublishedContent).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedContent).GetTypeInfo();
            else if (typeof(IPublishedElement).IsAssignableFrom(unwrappedTypeInfo))
                unwrappedTypeInfo = typeof(IPublishedElement).GetTypeInfo();

            var propertyGraphType = typeRegistry.Get(unwrappedTypeInfo) ?? typeof(StringGraphType).GetTypeInfo();
            propertyGraphType = WrapTypeInfo(propertyGraphType, type, propertyType.Mandatory, false);

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

        private static TypeInfo UnwrapTypeInfo(TypeInfo typeInfo)
        {
            var isNullable = typeInfo.IsNullable();
            if (isNullable)
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var enumerableArgument = GetEnumerableArgument(typeInfo);
            if (enumerableArgument != null)
                return enumerableArgument.GetTypeInfo();

            return typeInfo;
        }

        private static TypeInfo GetEnumerableArgument(TypeInfo typeInfo)
        {
            if (typeInfo == typeof(string) || typeof(JToken).IsAssignableFrom(typeInfo))
                return null;

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var enumerableInterface = typeInfo.ImplementedInterfaces.FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GenericTypeArguments[0].GetTypeInfo();
        }

        private TypeInfo WrapTypeInfo(TypeInfo graphType, TypeInfo typeInfo, bool isNonNull, bool isNonNullItem)
        {
            if (graphType == null)
                return null;

            var enumerableArgument = GetEnumerableArgument(typeInfo);

            if (typeInfo.IsValueType && typeInfo.IsNullable() == false || enumerableArgument != null &&
                (enumerableArgument.IsValueType && enumerableArgument.IsNullable() == false || isNonNullItem))
                graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            if (enumerableArgument != null)
                graphType = typeof(ListGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            if (isNonNull && typeof(NonNullGraphType).IsAssignableFrom(graphType) == false)
                graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            return graphType;
        }
    }
}
