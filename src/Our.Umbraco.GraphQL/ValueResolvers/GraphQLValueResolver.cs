using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public abstract class GraphQLValueResolver : IGraphQLValueResolver
    {
        public abstract Type GetGraphQLType(PublishedPropertyType propertyType);

        public abstract bool IsResolver(PublishedPropertyType propertyType);

        public virtual object Resolve(IPublishedElement owner, PublishedPropertyType propertyType, object value)
        {
            return value;
        }
    }
}
