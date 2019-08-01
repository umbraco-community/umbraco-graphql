using System;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [DefaultGraphQLValueResolver]
    public class GridValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return typeof(GridGraphType);
        }

        public override bool IsResolver(PublishedPropertyType propertyType)
        {
            return propertyType.DataType.EditorAlias == global::Umbraco.Core.Constants.PropertyEditors.Aliases.Grid;
        }
    }
}
