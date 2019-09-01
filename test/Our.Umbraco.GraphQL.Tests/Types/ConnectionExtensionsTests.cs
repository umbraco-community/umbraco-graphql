using System;
using System.Text;
using FluentAssertions;
using Our.Umbraco.GraphQL.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types
{
    public class ConnectionExtensionsTests
    {
        [Fact]
        public void ToConnection_WithCollection_ShouldHaveItems()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.Items.Should().Contain(items);
        }

        [Fact]
        public void ToConnection_WithCollection_EdgesShouldContainCollection()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.Edges.Should().SatisfyRespectively(
                x => x.Node.Should().Be(items[0]),
                x => x.Node.Should().Be(items[1])
            );
        }

        [Fact]
        public void ToConnection_WithCollection_EdgesShouldHaveOffsetAsEncodedCursor()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.Edges.Should().SatisfyRespectively(
                x => x.Cursor.Should().Be(ToCursor(new Id("1"))),
                x => x.Cursor.Should().Be(ToCursor(new Id("2")))
            );
        }

        [Fact]
        public void ToConnection_WithCollection_SetsPageInfoStartCursor()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.PageInfo.StartCursor.Should().Be(ToCursor(new Id("1")));
        }

        [Fact]
        public void ToConnection_WithCollection_SetsPageInfoEndCursor()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.PageInfo.EndCursor.Should().Be(ToCursor(new Id("2")));
        }

        [Fact]
        public void ToConnection_WithCollection_SetsPageInfoHasNextPage()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.PageInfo.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void ToConnection_WithCollection_SetsPageInfoHasPreviousPage()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id);

            connection.PageInfo.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public void ToConnection_WithTotalCount_ShouldSetTotalCount()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id, totalCount: items.Length);

            connection.TotalCount.Should().Be(items.Length);
        }

        [Fact]
        public void ToConnection_WithFirstArgumentLessThanZero_ThrowsArgumentException()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            Action action = () => items.ToConnection(x => x.Id, first: -1);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ToConnection_WithFirstArgument_OnlyReturnsFirst()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id, first: 1);

            connection.Edges.Should().OnlyContain(x => x.Node == items[0]);
        }

        [Fact]
        public void ToConnection_WithFirstArgument_SetsPageInfoHasNextPage()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id, first: 1);

            connection.PageInfo.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void ToConnection_WithAfterArgument_OnlyReturnsItemsAfter()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
                new Item {Id = new Id("3"), Name = "Item 3"},
                new Item {Id = new Id("4"), Name = "Item 4"},
                new Item {Id = new Id("5"), Name = "Item 5"},
            };

            var connection = items.ToConnection(x => x.Id, after: ToCursor(new Id("1")));

            connection.Edges.Should().NotContain(x => x.Node == items[0])
                .And.SatisfyRespectively(
                    x => x.Node.Should().Be(items[1]),
                    x => x.Node.Should().Be(items[2]),
                    x => x.Node.Should().Be(items[3]),
                    x => x.Node.Should().Be(items[4])
                );
        }

        [Fact]
        public void ToConnection_WithLastArgumentLessThanZero_ThrowsArgumentException()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            Action action = () => items.ToConnection(x => x.Id, last: -1);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ToConnection_WithFirstAndAfterArgument_OnlyReturnsFirstItemsAfter()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
                new Item {Id = new Id("3"), Name = "Item 3"},
                new Item {Id = new Id("4"), Name = "Item 4"},
                new Item {Id = new Id("5"), Name = "Item 5"},
            };

            var connection = items.ToConnection(x => x.Id, first: 2, after: ToCursor(new Id("2")));

            connection.Edges.Should().NotContain(new[] {items[0], items[1], items[4]})
                .And.SatisfyRespectively(
                    x => x.Node.Should().Be(items[2]),
                    x => x.Node.Should().Be(items[3])
                );
        }

        [Fact]
        public void ToConnection_WithLastArgument_OnlyReturnsLast()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id, last: 1);

            connection.Edges.Should().OnlyContain(x => x.Node == items[1]);
        }

        [Fact]
        public void ToConnection_WithBeforeArgument_OnlyReturnsItemsBefore()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
                new Item {Id = new Id("3"), Name = "Item 3"},
                new Item {Id = new Id("4"), Name = "Item 4"},
                new Item {Id = new Id("5"), Name = "Item 5"},
            };

            var connection = items.ToConnection(x => x.Id, before: ToCursor(new Id("5")));

            connection.Edges.Should().NotContain(x => x.Node == items[4])
                .And.SatisfyRespectively(
                    x => x.Node.Should().Be(items[0]),
                    x => x.Node.Should().Be(items[1]),
                    x => x.Node.Should().Be(items[2]),
                    x => x.Node.Should().Be(items[3])
                );
        }

        [Fact]
        public void ToConnection_WithLastArgument_SetsPageInfoHasPreviousPage()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
            };

            var connection = items.ToConnection(x => x.Id, last: 1);

            connection.PageInfo.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void ToConnection_WithLastAndBeforeArgument_OnlyReturnsLastItemsBefore()
        {
            var items = new[]
            {
                new Item {Id = new Id("1"), Name = "Item 1"},
                new Item {Id = new Id("2"), Name = "Item 2"},
                new Item {Id = new Id("3"), Name = "Item 3"},
                new Item {Id = new Id("4"), Name = "Item 4"},
                new Item {Id = new Id("5"), Name = "Item 5"},
            };

            var connection = items.ToConnection(x => x.Id, last: 2, before: ToCursor(new Id("5")));

            connection.Edges.Should().NotContain(new[] {items[0], items[1], items[4]})
                .And.SatisfyRespectively(
                    x => x.Node.Should().Be(items[2]),
                    x => x.Node.Should().Be(items[3])
                );
        }

        private static string ToCursor(Id id)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"connection:{id}"));
        }

        private class Item
        {
            public Id Id { get; set; }
            public string Name { get; set; }
        }
    }
}
