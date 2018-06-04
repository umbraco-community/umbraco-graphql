using System;
using System.Collections.Generic;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class FloatValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(float)
                ? typeof(FloatGraphType)
                : typeof(ListGraphType<FloatGraphType>);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(float) ||
                   propertyType.ClrType == typeof(IEnumerable<float>);
        }
    }
}
