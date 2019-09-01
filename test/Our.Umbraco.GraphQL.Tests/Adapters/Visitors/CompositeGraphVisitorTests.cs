using GraphQL.Types;
using NSubstitute;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Visitors
{
    public class CompositeGraphVisitorTests
    {
        private CompositeGraphVisitor CreateSUT(params GraphVisitor[] visitors) => new CompositeGraphVisitor(visitors);

        [Fact]
        public void Visit_WithInputObjectGraphType_CallsAllVisitors()
        {
            var visitor1 = Substitute.For<GraphVisitor>();
            var visitor2 = Substitute.For<GraphVisitor>();
            var visitor = CreateSUT(visitor1, visitor2);
            var graphType = new InputObjectGraphType();

            visitor.Visit(graphType);

            visitor1.Received(1).Visit(Arg.Is(graphType));
            visitor2.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Visit_WithInterfaceGraphType_CallsAllVisitors()
        {
            var visitor1 = Substitute.For<GraphVisitor>();
            var visitor2 = Substitute.For<GraphVisitor>();
            var visitor = CreateSUT(visitor1, visitor2);
            var graphType = new InterfaceGraphType();

            visitor.Visit(graphType);

            visitor1.Received(1).Visit(Arg.Is(graphType));
            visitor2.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Visit_WithObjectGraphType_CallsAllVisitors()
        {
            var visitor1 = Substitute.For<GraphVisitor>();
            var visitor2 = Substitute.For<GraphVisitor>();
            var visitor = CreateSUT(visitor1, visitor2);
            var graphType = new ObjectGraphType();

            visitor.Visit(graphType);

            visitor1.Received(1).Visit(Arg.Is(graphType));
            visitor2.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Visit_WithSchema_CallsAllVisitors()
        {
            var visitor1 = Substitute.For<GraphVisitor>();
            var visitor2 = Substitute.For<GraphVisitor>();
            var visitor = CreateSUT(visitor1, visitor2);
            var schema = new Schema();

            visitor.Visit(schema);

            visitor1.Received(1).Visit(Arg.Is(schema));
            visitor2.Received(1).Visit(Arg.Is(schema));
        }
    }
}
