using System;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay;

namespace Our.Umbraco.GraphQL.Adapters.Types.Relay
{
    public class ConnectionGraphType<T> : ConnectionType<T> where T : IGraphType
    {
        public ConnectionGraphType()
        {
            var text = typeof(T).GraphQLName();

            Name = text + "ConnectionGraph";
            Description = $"A connection from an object to a list of objects of type `{text}`.";
        }
    }

    public class ConnectionGraphType : ConnectionGraphType<ObjectGraphType>
    {
        public ConnectionGraphType(IGraphType graphType)
        {
            ResolvedType = graphType ?? throw new ArgumentNullException(nameof(graphType));

            Name = $"{graphType.Name}ConnectionGraph";
            Description = $"A connection from an object to a list of objects of type `{graphType.Name}`.";

            GetField("edges").ResolvedType = new ListGraphType(new EdgeGraphType(graphType));
            GetField("items").ResolvedType = new ListGraphType(graphType);
        }

        public IGraphType ResolvedType { get; }
    }
}
