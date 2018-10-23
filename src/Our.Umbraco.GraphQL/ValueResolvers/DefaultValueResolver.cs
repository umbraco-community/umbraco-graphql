using System;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class DefaultValueResolver : GraphQLValueResolver
    {
        public override object Resolve(PublishedPropertyType propertyType, object value)
        {
            return value?.ToString();
        }

        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return typeof(StringGraphType);
        }

        public override bool IsResolver(PublishedPropertyType propertyType)
        {
            return false;
        }
    }
}
