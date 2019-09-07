using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class GuidGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var graphType = new GuidGraphType();
            graphType.Name.Should().Be("Guid");
        }

        [Fact]
        public void Ctor_SetsDescription()
        {
            var graphType = new GuidGraphType();
            graphType.Description.Should().Be("Globally Unique Identifier.");
        }
    }
}
