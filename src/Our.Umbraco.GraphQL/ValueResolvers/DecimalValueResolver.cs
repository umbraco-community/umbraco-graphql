using System;
using System.Collections.Generic;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [DefaultGraphQLValueResolver]
    public class DecimalValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(decimal) ||
                   propertyType.ClrType == typeof(double)
                ? typeof(DecimalGraphType)
                : typeof(ListGraphType<DecimalGraphType>);
        }

        public override bool IsResolver(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(decimal) ||
                   propertyType.ClrType == typeof(double) ||
                   propertyType.ClrType == typeof(IEnumerable<decimal>) ||
                   propertyType.ClrType == typeof(IEnumerable<double>);
        }
    }
}
