using System.Web;
using FluentAssertions;
using GraphQL.Language.AST;
using Microsoft.AspNetCore.Html;
using Our.Umbraco.GraphQL.Adapters.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class HtmlGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var htmlGraphType = new HtmlGraphType();

            htmlGraphType.Name.Should().Be("HTML");
        }

        [Fact]
        public void Ctor_SetsDescription()
        {
            var htmlGraphType = new HtmlGraphType();

            htmlGraphType.Description.Should().Be("A string containing HTML code.");
        }

        [Fact]
        public void Serialize_WithHtmlString_ReturnsValueToString()
        {
            var htmlGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";
            var htmlString = new HtmlString(value);

            var serialized = htmlGraphType.Serialize(htmlString);

            serialized.Should().BeOfType<string>().Which.Should().Be(value);
        }

        [Fact]
        public void Serialize_WithNull_ReturnsNull()
        {
            var htmlGraphType = new HtmlGraphType();

            var serialized = htmlGraphType.Serialize(null);

            serialized.Should().BeNull();
        }

        [Fact]
        public void ParseValue_WithValue_ReturnsHtmlString()
        {
            var htmlGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";

            var parsed = htmlGraphType.ParseValue(value);

            parsed.Should().BeAssignableTo<HtmlString>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseValue_WithNull_ReturnsNull()
        {
            var htmlGraphType = new HtmlGraphType();

            var parsed = htmlGraphType.ParseValue(null);

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseLiteral_WithStringValue_ReturnsHtmlString()
        {
            var htmlGraphType = new HtmlGraphType();
            var value = "<div>Some HTML</div>";

            var parsed = htmlGraphType.ParseLiteral(new StringValue(value));

            parsed.Should().BeAssignableTo<HtmlString>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseLiteral_WithNull_ReturnsNull()
        {
            var htmlGraphType = new HtmlGraphType();

            var parsed = htmlGraphType.ParseLiteral(null);

            parsed.Should().BeNull();
        }
    }
}
