using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types.Relay
{
    public class PageInfoGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var graphType = new PageInfoGraphType();

            graphType.Name.Should().Be("PageInfo");
        }

        [Fact]
        public void Ctor_AddsFields()
        {
            var graphType = new PageInfoGraphType();

            graphType.Fields.Should().Contain(field => field.Name == "endCursor")
                .And.Contain(field => field.Name == "hasNextPage")
                .And.Contain(field => field.Name == "hasPreviousPage")
                .And.Contain(field => field.Name == "startCursor");
        }
    }
}
