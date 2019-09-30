using System;
using System.Web;
using FluentAssertions;
using GraphQL.Language.AST;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class HtmlGraphTypeTests
    {
        [Fact]
        public void Serialize_WithHtmlString_ReturnsValueToString()
        {
            var idGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";
            var htmlString = new HtmlString(value);

            var serialized = idGraphType.Serialize(htmlString);

            serialized.Should().BeOfType<string>().Which.Should().Be(value);
        }

        [Fact]
        public void Serialize_WithNull_ReturnsNull()
        {
            var idGraphType = new HtmlGraphType();

            var serialized = idGraphType.Serialize(null);

            serialized.Should().BeNull();
        }

        [Fact]
        public void ParseValue_WithValue_ReturnsHtmlString()
        {
            var idGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";

            var parsed = idGraphType.ParseValue(value);

            parsed.Should().BeAssignableTo<IHtmlString>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseValue_WithNull_ReturnsNull()
        {
            var idGraphType = new HtmlGraphType();

            var parsed = idGraphType.ParseValue(null);

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseLiteral_WithStringValue_ReturnsHtmlString()
        {
            var idGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";

            var parsed = idGraphType.ParseLiteral(new StringValue(value));

            parsed.Should().BeAssignableTo<IHtmlString>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseLiteral_WithNull_ReturnsNull()
        {
            var idGraphType = new HtmlGraphType();

            var parsed = idGraphType.ParseLiteral(null);

            parsed.Should().BeNull();
        }
    }
}
