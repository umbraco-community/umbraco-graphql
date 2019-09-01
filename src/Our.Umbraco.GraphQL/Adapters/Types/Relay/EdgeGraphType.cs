using System;
using GraphQL.Types;
using GraphQL.Types.Relay;

namespace Our.Umbraco.GraphQL.Adapters.Types.Relay
{
    public class EdgeGraphType<T> : EdgeType<T> where T : IGraphType
    {
    }

    public class EdgeGraphType : EdgeGraphType<ObjectGraphType>
    {
        public EdgeGraphType(IGraphType graphType)
        {
            ResolvedType = graphType ?? throw new ArgumentNullException(nameof(graphType));

            Name = $"{graphType.Name}Edge";
            Description = $"An edge in a connection from an object to another object of type `{graphType.Name}`.";

            GetField("node").ResolvedType = graphType;
        }

        public IGraphType ResolvedType { get; }
    }
}
