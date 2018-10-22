using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Our.Umbraco.GraphQL.ValueResolvers;
using System;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedPropertyFieldType : FieldType
    {
        public PublishedPropertyFieldType(PublishedContentType contentType, PropertyType propertyType, GraphQLValueResolverCollection graphQLValueResolvers)
        {
            PublishedPropertyType publishedPropertyType = contentType.GetPropertyType(propertyType.Alias);
            IGraphQLValueResolver foundResolver = GetValueResolver(contentType, propertyType, publishedPropertyType, graphQLValueResolvers);

            Type propertyGraphType = foundResolver.GetGraphQLType(publishedPropertyType);

            if (propertyType.Mandatory)
            {
                propertyGraphType = typeof(NonNullGraphType<>).MakeGenericType(propertyGraphType);
            }

            Type = propertyGraphType;
            Name = publishedPropertyType.Alias.ToCamelCase();
            Description = propertyType.Description;
            Resolver = new FuncFieldResolver<IPublishedContent, object>(context =>
            {
                var userContext = (UmbracoGraphQLContext)context.UserContext;
                IPublishedProperty publishedProperty = context.Source.GetProperty(publishedPropertyType.Alias);
                return foundResolver.Resolve(context.Source, publishedPropertyType, publishedProperty.GetValue(userContext.Culture));
            });
        }

        private static IGraphQLValueResolver GetValueResolver(PublishedContentType contentType, PropertyType propertyType, PublishedPropertyType publishedPropertyType, GraphQLValueResolverCollection graphQLValueResolvers)
        {
            IGraphQLValueResolver foundResolver = null;
            bool isDefault = false;
            foreach (IGraphQLValueResolver resolver in graphQLValueResolvers.Where(r => r.IsResolver(publishedPropertyType)))
            {
                if (foundResolver == null)
                {
                    foundResolver = resolver;
                    isDefault = graphQLValueResolvers.IsDefault(resolver);
                    continue;
                }

                bool currentIsDefault = graphQLValueResolvers.IsDefault(resolver);
                if (!isDefault && currentIsDefault)
                {
                    // previous was non-default, ignore default
                    continue;
                }

                if (isDefault && !currentIsDefault)
                {
                    // previous was default and current wasn't default, replace by non-default
                    foundResolver = resolver;
                    isDefault = false;
                    continue;
                }

                // previous was non-default - bad
                throw new InvalidOperationException($"Type '{resolver.GetType().FullName}' cannot be an IGraphQLValueResolver" +
                    $" for property '{propertyType.Alias}' of content type '{contentType.Alias}' because type '{foundResolver.GetType().FullName}' has already been detected as a resolver" +
                    $" for that property, and only one converter can exist for a resolver.");
            }

            return foundResolver ?? graphQLValueResolvers.FallbackResolver;
        }
    }
}
