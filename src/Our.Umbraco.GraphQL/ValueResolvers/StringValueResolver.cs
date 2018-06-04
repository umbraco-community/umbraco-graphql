using System;
using System.Collections.Generic;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class StringValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(string) 
                ? typeof(StringGraphType)
                : typeof(ListGraphType<StringGraphType>);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(string) ||
                   propertyType.ClrType == typeof(IEnumerable<string>);
        }
    }
}
