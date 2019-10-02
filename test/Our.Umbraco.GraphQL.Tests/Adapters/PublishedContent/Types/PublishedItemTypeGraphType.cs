using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Umbraco.Core.Models.PublishedContent;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class PublishedItemTypeGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var sut = new PublishedItemTypeGraphType();

            sut.Name.Should().Be("PublishedItemType");
        }
        [Fact]
        public void Ctor_SetsDescription()
        {
            var sut = new PublishedItemTypeGraphType();

            sut.Description.Should().Be("The type of published element.");
        }

        [Theory]
        [InlineData("CONTENT", PublishedItemType.Content)]
        [InlineData("ELEMENT", PublishedItemType.Element)]
        [InlineData("MEDIA", PublishedItemType.Media)]
        [InlineData("MEMBER", PublishedItemType.Member)]
        [InlineData("UNKNOWN", PublishedItemType.Unknown)]
        public void Ctor_AddsFields(string field, PublishedItemType value)
        {
            var sut = new PublishedItemTypeGraphType();

            sut.Values.Should().Contain(x => x.Name == field)
                .Which.Value.Should().Be(value);
        }
    }
}
