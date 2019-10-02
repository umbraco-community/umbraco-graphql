using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Umbraco.Core.Models.PublishedContent;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class UrlModeGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var sut = new UrlModeGraphType();

            sut.Name.Should().Be("UrlMode");
        }
        [Fact]
        public void Ctor_SetsDescription()
        {
            var sut = new UrlModeGraphType();

            sut.Description.Should()
                .Be("Specifies the type of urls that the url provider should produce, Auto is the default.");
        }

        [Theory]
        [InlineData("ABSOLUTE", UrlMode.Absolute)]
        [InlineData("AUTO", UrlMode.Auto)]
        [InlineData("DEFAULT", UrlMode.Default)]
        [InlineData("RELATIVE", UrlMode.Relative)]
        public void Ctor_AddsFields(string field, UrlMode value)
        {
            var sut = new UrlModeGraphType();

            sut.Values.Should().Contain(x => x.Name == field)
                .Which.Value.Should().Be(value);
        }
    }
}
