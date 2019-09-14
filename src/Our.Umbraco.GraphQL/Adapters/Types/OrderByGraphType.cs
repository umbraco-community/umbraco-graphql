using System;
using System.Reflection;
using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class OrderByGraphType : EnumerationGraphType
    {
        public OrderByGraphType(IComplexGraphType graphType)
        {
            if (graphType == null) throw new ArgumentNullException(nameof(graphType));

            Name = $"{graphType.Name}Order";

            foreach (var field in graphType.Fields)
            {
                Type fieldType = null;
                if (field.Type != null)
                {
                    fieldType = field.Type;
                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
                        fieldType = fieldType.GenericTypeArguments[0];
                }

                if (field.ResolvedType != null)
                {
                    fieldType = field.ResolvedType is NonNullGraphType nonNullGraphType
                        ? nonNullGraphType.ResolvedType.GetType()
                        : field.ResolvedType.GetType();
                }

                if (fieldType == null || typeof(ScalarGraphType).IsAssignableFrom(fieldType) == false) continue;

                var name = field.GetMetadata<MethodInfo>(nameof(MethodInfo))?.Name ?? field.Name;

                AddValue($"{field.Name}_ASC", $"Order by {field.Name} ascending.",
                    new OrderBy(name, SortOrder.Ascending, field.Resolver.Resolve));
                AddValue($"{field.Name}_DESC", $"Order by {field.Name} descending.",
                    new OrderBy(name, SortOrder.Descending, field.Resolver.Resolve));
            }
        }
    }

    public class OrderByGraphType<TNodeType> : OrderByGraphType where TNodeType : IComplexGraphType
    {
        public OrderByGraphType() : base((IComplexGraphType) typeof(TNodeType).BuildNamedType())
        {
        }
    }
}
