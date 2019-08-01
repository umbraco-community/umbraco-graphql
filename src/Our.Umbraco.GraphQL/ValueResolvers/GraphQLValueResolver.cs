using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public abstract class GraphQLValueResolver : IGraphQLValueResolver
    {
        public abstract Type GetGraphQLType(IPublishedPropertyType propertyType);

        public abstract bool IsResolver(IPublishedPropertyType propertyType);

        public virtual object Resolve(IPublishedElement owner, IPublishedPropertyType propertyType, object value)
        {
            return value;
        }
    }
}