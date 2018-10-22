using System;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [DefaultGraphQLValueResolver]
    public class DefaultValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return typeof(StringGraphType);
        }

        public override bool IsResolver(PublishedPropertyType propertyType)
        {
            return false;
        }

        public override object Resolve(IPublishedElement owner, PublishedPropertyType propertyType, object value)
        {
            return value?.ToString();
        }
    }
}
