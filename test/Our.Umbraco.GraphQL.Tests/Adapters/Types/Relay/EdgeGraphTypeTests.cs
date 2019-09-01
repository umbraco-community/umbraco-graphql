using FluentAssertions;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types.Relay
{
    public class EdgeGraphTypeTests
    {
        [Fact]
        public void Ctor_WithInstance_SetsName()
        {
            var graphType = new EdgeGraphType(new ItemGraphType());

            graphType.Name.Should().Be("ItemEdge");
        }

        [Fact]
        public void Ctor_GenericClassWithType_SetsName()
        {
            var graphType = new EdgeGraphType<ItemGraphType>();

            graphType.Name.Should().Be("ItemEdge");
        }

        [Fact]
        public void Ctor_WithInstance_SetsResolvedType()
        {
            var itemGraphType = new ItemGraphType();
            var graphType = new EdgeGraphType(itemGraphType);

            graphType.ResolvedType.Should().Be(itemGraphType);
        }

        [Fact]
        public void Ctor_WithInstance_AddsFields()
        {
            var graphType = new EdgeGraphType(new ItemGraphType());

            graphType.Fields.Should().SatisfyRespectively(
                field => field.Name.Should().Be("cursor"),
                field => field.Name.Should().Be("node")
            );
        }

        [Fact]
        public void Ctor_WithInstance_SetsNodeResolvedType()
        {
            var graphType = new EdgeGraphType(new ItemGraphType());

            graphType.Fields.Should().Contain(x => x.Name == "node")
                .Which.ResolvedType.Should().BeAssignableTo<ItemGraphType>();
        }

        private class ItemGraphType : ObjectGraphType
        {
            public ItemGraphType()
            {
                Name = "Item";
            }
        }
    }
}
