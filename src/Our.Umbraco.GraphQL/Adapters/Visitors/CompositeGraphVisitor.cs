using System;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Visitors
{
    public class CompositeGraphVisitor : GraphVisitor
    {
        private readonly IGraphVisitor[] _visitors;

        public CompositeGraphVisitor(params IGraphVisitor[] visitors)
        {
            _visitors = visitors ?? throw new ArgumentNullException(nameof(visitors));
        }

        public override void Visit(EnumerationGraphType graphType)
        {
            foreach (var visitor in _visitors) visitor.Visit(graphType);
        }

        public override void Visit(IInputObjectGraphType graphType)
        {

            foreach (var visitor in _visitors) visitor.Visit(graphType);
        }

        public override void Visit(IInterfaceGraphType graphType)
        {
            foreach (var visitor in _visitors) visitor.Visit(graphType);
        }

        public override void Visit(IObjectGraphType graphType)
        {
            foreach (var visitor in _visitors) visitor.Visit(graphType);
        }

        public override void Visit(ISchema schema)
        {
            foreach (var visitor in _visitors) visitor.Visit(schema);
        }
    }
}
