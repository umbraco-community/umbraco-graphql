using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public interface IGraphQLValueResolver : IDiscoverable
    {
        Type GetGraphQLType(IPublishedPropertyType propertyType);
        bool IsResolver(IPublishedPropertyType propertyType);
        object Resolve(IPublishedElement owner, IPublishedPropertyType propertyType, object value);
    }
}
