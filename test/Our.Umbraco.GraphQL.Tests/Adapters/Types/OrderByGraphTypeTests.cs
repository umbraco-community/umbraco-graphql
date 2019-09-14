using System;
using FluentAssertions;
using GraphQL.Resolvers;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Types;
using Xunit;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class OrderByGraphTypeTests
    {
        [Fact]
        public void Ctor_WithGraphType_SetsNameToGraphTypeOrder()
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Name.Should().Be("PersonOrder");
        }

        [Fact]
        public void Ctor_WithNull_ThrowsArgumentNullException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new OrderByGraphType(null);

            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("graphType");
        }

        [Fact]
        public void Ctor_WithGenericType_SetsNameToGraphTypeOrder()
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Name.Should().Be("PersonOrder");
        }

        [Fact]
        public void Ctor_WithGenericType_AddsScalarFieldsAsValues()
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == "id_ASC")
                .And.Contain(x => x.Name == "id_DESC")
                .And.Contain(x => x.Name == "name_ASC")
                .And.Contain(x => x.Name == "name_DESC")
                .And.Contain(x => x.Name == "phoneNumber_ASC")
                .And.Contain(x => x.Name == "phoneNumber_DESC");
        }

        [Fact]
        public void Ctor_WithGenericType_AddsEnumFieldsAsValues()
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == "private_ASC")
                .And.Contain(x => x.Name == "private_DESC");
        }

        [Fact]
        public void Ctor_WithGenericType_DoesNotAddComplexFieldsAsValues()
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .NotContain(x => x.Name == "location_ASC")
                .And.NotContain(x => x.Name == "location_DESC");
        }

        [Fact]
        public void Ctor_WithGraphType_AddsScalarFieldsAsValues()
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .Contain(x => x.Name == "id_ASC")
                .And.Contain(x => x.Name == "id_DESC")
                .And.Contain(x => x.Name == "name_ASC")
                .And.Contain(x => x.Name == "name_DESC")
                .And.Contain(x => x.Name == "phoneNumber_ASC")
                .And.Contain(x => x.Name == "phoneNumber_DESC");
        }

        [Fact]
        public void Ctor_WithGraphType_AddsEnumFieldsAsValues()
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .Contain(x => x.Name == "private_ASC")
                .And.Contain(x => x.Name == "private_DESC");
        }

        [Fact]
        public void Ctor_WithGraphType_DoesNotAddComplexFieldsAsValues()
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .NotContain(x => x.Name == "location_ASC")
                .And.NotContain(x => x.Name == "location_DESC");
        }

        [Theory]
        [InlineData("name_ASC", "Order by name ascending.")]
        [InlineData("name_DESC", "Order by name descending.")]
        [InlineData("id_ASC", "Order by id ascending.")]
        [InlineData("id_DESC", "Order by id descending.")]
        public void Ctor_WithGraphType_SetsDescription(string name, string description)
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .Contain(x => x.Name == name)
                .Which.Description.Should().Be(description);
        }

        [Theory]
        [InlineData("name_ASC", "Order by name ascending.")]
        [InlineData("name_DESC", "Order by name descending.")]
        [InlineData("id_ASC", "Order by id ascending.")]
        [InlineData("id_DESC", "Order by id descending.")]
        public void Ctor_WithGenericType_SetsDescription(string name, string description)
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == name)
                .Which.Description.Should().Be(description);
        }

        [Theory]
        [InlineData("name_ASC", "name")]
        [InlineData("name_DESC", "name")]
        [InlineData("id_ASC", "id")]
        [InlineData("id_DESC", "id")]
        public void Ctor_WithGraphType_SetsValueField(string field, string name)
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .Contain(x => x.Name == field)
                .Which.Value.Should().BeOfType<OrderBy>()
                .Which.Field.Should().Be(name);
        }

        [Theory]
        [InlineData("name_ASC", SortOrder.Ascending)]
        [InlineData("name_DESC", SortOrder.Descending)]
        [InlineData("id_ASC", SortOrder.Ascending)]
        [InlineData("id_DESC", SortOrder.Descending)]
        public void Ctor_WithGraphType_SetsValueDirection(string field, SortOrder direction)
        {
            var sut = new OrderByGraphType(new PersonGraphType());

            sut.Values.Should()
                .Contain(x => x.Name == field)
                .Which.Value.Should().BeOfType<OrderBy>()
                .Which.Direction.Should().Be(direction);
        }

        [Theory]
        [InlineData("name_ASC", "name")]
        [InlineData("name_DESC", "name")]
        [InlineData("id_ASC", "id")]
        [InlineData("id_DESC", "id")]
        public void Ctor_WithGenericType_SetsValueField(string field, string name)
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == field)
                .Which.Value.Should().BeOfType<OrderBy>()
                .Which.Field.Should().Be(name);
        }

        [Theory]
        [InlineData("name_ASC", SortOrder.Ascending)]
        [InlineData("name_DESC", SortOrder.Descending)]
        [InlineData("id_ASC", SortOrder.Ascending)]
        [InlineData("id_DESC", SortOrder.Descending)]
        public void Ctor_WithGenericType_SetsValueDirection(string field, SortOrder direction)
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == field)
                .Which.Value.Should().BeOfType<OrderBy>()
                .Which.Direction.Should().Be(direction);
        }

        [Theory]
        [InlineData("name_ASC", "John")]
        [InlineData("name_DESC", "John")]
        [InlineData("phoneNumber_ASC", "12345678")]
        [InlineData("phoneNumber_DESC", "12345678")]
        [InlineData("private_ASC", Private.No)]
        [InlineData("private_DESC", Private.No)]
        public void Ctor_WithGenericType_SetsValueResolve(string field, object expectedValue)
        {
            var sut = new OrderByGraphType<PersonGraphType>();

            sut.Values.Should()
                .Contain(x => x.Name == field)
                .Which.Value.Should().BeOfType<OrderBy>()
                .Which.Resolve((object)null).Should().Be(expectedValue);
        }

        private class PersonGraphType : ObjectGraphType
        {
            public PersonGraphType()
            {
                Name = "Person";

                Field<NonNullGraphType<IdGraphType>>("id", resolve: ctx => new Id("1"));
                Field<NonNullGraphType<StringGraphType>>("name", resolve: ctx => "John");
                Field<StringGraphType>("phoneNumber", resolve: ctx => "12345678");
                Field<ObjectGraphType>("location", resolve: ctx => null);
                base.AddField(
                    new FieldType
                    {
                        Name = "private",
                        ResolvedType = new EnumerationGraphType<Private>(),
                        Resolver = new FuncFieldResolver<Private>(ctx => Private.No)
                    }
                );
            }
        }

        private enum Private
        {
            Unknown,
            Yes,
            No
        }
    }
}
