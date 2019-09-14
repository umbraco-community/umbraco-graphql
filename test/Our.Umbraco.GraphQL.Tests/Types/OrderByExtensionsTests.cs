using FluentAssertions;
using Our.Umbraco.GraphQL.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types
{
    public class OrderByExtensionsTests
    {
        [Fact]
        public void OrderBy_WithSingleOrderByAsc_OrdersByAsc()
        {
            var collection = new[]
            {
                new Item { Name = "c"},
                new Item { Name = "b" },
                new Item { Name = "n"},
                new Item { Name = "a"},
            };

            var result = collection.OrderBy(new[]
            {
                new OrderBy("Name", SortOrder.Ascending, x => ((Item) x.Source).Name)
            });

            result.Should().BeInAscendingOrder(x => x.Name);
        }

        [Fact]
        public void OrderBy_WithSingleOrderByDesc_OrdersByDesc()
        {
            var collection = new[]
            {
                new Item { Name = "c"},
                new Item { Name = "b" },
                new Item { Name = "n"},
                new Item { Name = "a"},
            };

            var result = collection.OrderBy(new[]
            {
                new OrderBy("Name", SortOrder.Descending, x => ((Item) x.Source).Name)
            });

            result.Should().BeInDescendingOrder(x => x.Name);
        }

        [Fact]
        public void OrderBy_WithMultipleOrderByAscending_OrdersByFirstThenNext()
        {
            var collection = new[]
            {
                new Item { Id = new Id("1"), Name = "c", Stock = 1 },
                new Item { Id = new Id("2"), Name = "b", Stock = 2 },
                new Item { Id = new Id("3"), Name = "n", Stock = 1 },
                new Item { Id = new Id("4"), Name = "a", Stock = 4 },
            };

            var result = collection.OrderBy(new[]
            {
                new OrderBy("Stock", SortOrder.Ascending, x => ((Item) x.Source).Stock),
                new OrderBy("Name", SortOrder.Ascending, x => ((Item) x.Source).Name),
            });

            result.Should().SatisfyRespectively(
                x => x.Name.Should().Be("c"),
                x => x.Name.Should().Be("n"),
                x => x.Name.Should().Be("b"),
                x => x.Name.Should().Be("a"));
        }

        [Fact]
        public void OrderBy_WithMultipleOrderByAscendingThenDescending_OrdersByFirstThenNext()
        {
            var collection = new[]
            {
                new Item { Id = new Id("1"), Name = "c", Stock = 1 },
                new Item { Id = new Id("2"), Name = "b", Stock = 2 },
                new Item { Id = new Id("3"), Name = "n", Stock = 1 },
                new Item { Id = new Id("4"), Name = "a", Stock = 4 },
            };

            var result = collection.OrderBy(new[]
            {
                new OrderBy("Stock", SortOrder.Ascending, x => ((Item) x.Source).Stock),
                new OrderBy("Name", SortOrder.Descending, x => ((Item) x.Source).Name),
            });

            result.Should().SatisfyRespectively(
                x => x.Name.Should().Be("n"),
                x => x.Name.Should().Be("c"),
                x => x.Name.Should().Be("b"),
                x => x.Name.Should().Be("a"));
        }

        private class Item
        {
            public Id Id { get; set; }
            public string Name { get; set; }
            public int Stock { get; set; }
        }
    }
}
