using System;
using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Models;

namespace Our.Umbraco.GraphQL.Types
{
    internal class OrderByGraphType<T> : EnumerationGraphType where T : GraphType
    {
        public OrderByGraphType(IComplexGraphType graphType)
        {
            Type = typeof(T);
            Name = $"{Type.GraphQLName()}Order";

            foreach (var field in graphType.Fields)
            {
                var namedType = field.Type.GetNamedType();
                if (namedType == typeof(EnumerationGraphType) ||
                    namedType.IsGenericType &&
                    namedType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>) ||
                    namedType == typeof(BooleanGraphType) ||
                    namedType == typeof(IntGraphType) ||
                    namedType == typeof(FloatGraphType) ||
                    namedType == typeof(DateGraphType) ||
                    namedType == typeof(StringGraphType) ||
                    namedType == typeof(IdGraphType))
                {
                    AddValue($"{field.Name}_ASC", "", new OrderBy(field.Name, SortOrder.Ascending));
                    AddValue($"{field.Name}_DESC", "", new OrderBy(field.Name, SortOrder.Descending));
                }
            }
        }

        public Type Type { get; }

        public override string CollectTypes(TypeCollectionContext context)
        {
            var innerType = context.ResolveType(Type);
            var name = innerType.CollectTypes(context);
            context.AddType(name, innerType, context);
            return Name;
        }
    }
}
