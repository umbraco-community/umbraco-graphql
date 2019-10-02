using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.Types;
using Umbraco.Web.Models;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class LinkTypeGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var sut = new LinkTypeGraphType();

            sut.Name.Should().Be("LinkType");
        }

        [Theory]
        [InlineData("CONTENT", LinkType.Content)]
        [InlineData("EXTERNAL", LinkType.External)]
        [InlineData("MEDIA", LinkType.Media)]
        public void Ctor_AddsFields(string field, LinkType value)
        {
            var sut = new LinkTypeGraphType();

            sut.Values.Should().Contain(x => x.Name == field)
                .Which.Value.Should().Be(value);
        }
    }
}
