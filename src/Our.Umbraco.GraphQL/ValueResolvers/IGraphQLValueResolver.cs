using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public interface IGraphQLValueResolver
    {
        Type GetGraphQLType(PublishedPropertyType propertyType);
        bool IsResolver(PublishedPropertyType propertyType);
        //TODO: Provide context?
        object Resolve(PublishedPropertyType propertyType, object value);
    }
}
