using System;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public interface IGraphQLValueResolver
    {
        //TODO: Provide context?
        object Resolve(PublishedPropertyType propertyType, object value);

        Type GetGraphQLType(PublishedPropertyType propertyType);
        bool IsConverter(PublishedPropertyType propertyType);
    }
}
