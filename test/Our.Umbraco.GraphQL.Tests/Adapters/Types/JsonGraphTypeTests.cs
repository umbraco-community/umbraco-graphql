using FluentAssertions;
using Our.Umbraco.GraphQL.Adapters.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class JsonGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var jsonGraphType = new JsonGraphType();

            jsonGraphType.Name.Should().Be("JSON");
        }

        [Fact]
        public void Ctor_SetsDescription()
        {
            var jsonGraphType = new JsonGraphType();

            jsonGraphType.Description.Should().Be("The `JSON` scalar type represents JSON values as specified by [ECMA-404](http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf).");
        }
    }
}
