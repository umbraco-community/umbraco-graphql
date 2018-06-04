using System;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class GridValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return typeof(GridGraphType);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias == Constants.PropertyEditors.GridAlias;
        }
    }
}
