using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public interface IGraphQLValueResolver : IDiscoverable
    {
        Type GetGraphQLType(PublishedPropertyType propertyType);
        bool IsResolver(PublishedPropertyType propertyType);
        object Resolve(IPublishedElement owner, PublishedPropertyType propertyType, object value);
    }
}
