using System;
using FluentAssertions;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class LinkGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var sut = new LinkGraphType();

            sut.Name.Should().Be("Link");
        }

        [Theory]
        [InlineData("name", typeof(NonNullGraphType<StringGraphType>))]
        [InlineData("target", typeof(StringGraphType))]
        [InlineData("type", typeof(NonNullGraphType<LinkTypeGraphType>))]
        [InlineData("url", typeof(NonNullGraphType<StringGraphType>))]
        [InlineData("udi", typeof(UdiGraphType))]
        public void Ctor_AddsFields(string field, Type type)
        {
            var sut = new LinkGraphType();

            sut.Fields.Should().Contain(x => x.Name == field)
                .Which.Type.Should().BeAssignableTo(type);
        }
    }
}
