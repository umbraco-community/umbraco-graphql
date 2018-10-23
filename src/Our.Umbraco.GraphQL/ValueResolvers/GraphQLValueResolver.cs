using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [DefaultGraphQLValueResolver]
    public abstract class GraphQLValueResolver : IGraphQLValueResolver
    {
        public abstract Type GetGraphQLType(PublishedPropertyType propertyType);

        public virtual object Resolve(PublishedPropertyType propertyType, object value)
        {
            return value;
        }

        public abstract bool IsResolver(PublishedPropertyType propertyType);
    }
}
