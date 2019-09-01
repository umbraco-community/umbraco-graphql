using FluentAssertions;
using GraphQL.Types;
using Lucene.Net.Documents;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types.Relay
{
    public class ConnectionGraphTypeTests
    {

        [Fact]
        public void Ctor_WithInstance_SetsName()
        {
            var graphType = new ConnectionGraphType(new ItemGraphType());

            graphType.Name.Should().Be("ItemConnection");
        }

        [Fact]
        public void Ctor_GenericClassWithType_SetsName()
        {
            var graphType = new ConnectionGraphType<ItemGraphType>();

            graphType.Name.Should().Be("ItemConnection");
        }

        [Fact]
        public void Ctor_WithInstance_SetsResolvedType()
        {
            var itemGraphType = new ItemGraphType();
            var graphType = new ConnectionGraphType(itemGraphType);

            graphType.ResolvedType.Should().Be(itemGraphType);
        }

        [Fact]
        public void Ctor_WithInstance_AddsFields()
        {
            var graphType = new ConnectionGraphType(new ItemGraphType());

            graphType.Fields.Should().Contain(field => field.Name == "edges")
                .And.Contain(field => field.Name == "items")
                .And.Contain(field => field.Name == "pageInfo")
                .And.Contain(field => field.Name == "totalCount");
        }

        [Fact]
        public void Ctor_WithInstance_SetsEdgesResolvedType()
        {
            var graphType = new ConnectionGraphType(new ItemGraphType());

            graphType.Fields.Should().Contain(x => x.Name == "edges")
                .Which.ResolvedType.Should().BeAssignableTo<ListGraphType>()
                    .Which.ResolvedType.Should().BeAssignableTo<EdgeGraphType>()
                    .Which.ResolvedType.Should().BeAssignableTo<ItemGraphType>();
        }

        [Fact]
        public void Ctor_WithInstance_SetsItemsResolvedType()
        {
            var graphType = new ConnectionGraphType(new ItemGraphType());

            graphType.Fields.Should().Contain(x => x.Name == "items")
                .Which.ResolvedType.Should().BeAssignableTo<ListGraphType>()
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
