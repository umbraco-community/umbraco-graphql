using FluentAssertions;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types.PublishedContent
{
    public class ExtendQueryWithUmbracoQueryTests
    {
        [Fact]
        public void ExtendQueryWithUmbracoQuery_Umbraco_WhenCalled_ReturnsUmbracoQuery()
        {
            var extendQuery = new ExtendQueryWithUmbracoQuery();

            var expected = new UmbracoQuery();
            var actual = extendQuery.Umbraco(expected);

            actual.Should().Be(expected);
        }
    }
}
