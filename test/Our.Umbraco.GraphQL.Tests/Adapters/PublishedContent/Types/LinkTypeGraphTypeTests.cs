using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Umbraco.Core.Models;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class ContentVariationTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var sut = new ContentVariationGraphType();

            sut.Name.Should().Be("ContentVariation");
        }
        [Fact]
        public void Ctor_SetsDescription()
        {
            var sut = new ContentVariationGraphType();

            sut.Description.Should().Be("Indicates how values can vary.");
        }

        [Theory]
        [InlineData("NOTHING", ContentVariation.Nothing)]
        [InlineData("CULTURE", ContentVariation.Culture)]
        [InlineData("SEGMENT", ContentVariation.Segment)]
        [InlineData("CULTURE_AND_SEGMENT", ContentVariation.CultureAndSegment)]
        public void Ctor_AddsFields(string field, ContentVariation value)
        {
            var sut = new ContentVariationGraphType();

            sut.Values.Should().Contain(x => x.Name == field)
                .Which.Value.Should().Be(value);
        }
    }
}
