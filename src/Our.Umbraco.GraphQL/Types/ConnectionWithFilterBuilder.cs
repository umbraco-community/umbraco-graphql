using GraphQL.Builders;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public static class ConnectionWithFilterBuilder
    {
        public static ConnectionBuilder<TNodeType, object>
            FilteredConnection<TNodeType>(this ComplexGraphType<object> graphType)
            where TNodeType : GraphType
        {
            return FilteredConnection<TNodeType, object>(graphType);
        }
    
        public static ConnectionBuilder<TNodeType, TSourceType>
            FilteredConnection<TNodeType, TSourceType>(this ComplexGraphType<TSourceType> graphType)
            where TNodeType : GraphType
        {
            var builder = graphType.Connection<TNodeType>();
            builder.FieldType.Arguments.Add(
                new QueryArgument(typeof(FilterGraphType<TNodeType>))
                {
                    Name = "filter",
                    ResolvedType = new FilterGraphType<TNodeType>(graphType)
                }
            );
            builder.FieldType.Arguments.Add(
                new QueryArgument(typeof(ListGraphType<NonNullGraphType<OrderByGraphType<TNodeType>>>))
                {
                    Name = "orderBy",
                    ResolvedType = new ListGraphType(new NonNullGraphType(new OrderByGraphType<TNodeType>(graphType)))
                }
            );
            return builder;
        }
    }
}
