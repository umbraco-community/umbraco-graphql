using System;
using FluentAssertions;
using GraphQL.Language.AST;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class IdGraphTypeTests
    {
        [Fact]
        public void Serialize_WithId_ReturnsValueToString()
        {
            var idGraphType = new IdGraphType();
            var value = "05087385-5095-448D-AA90-4B9AD3C1CA3F";
            var id = new Id(value);

            var serialized = idGraphType.Serialize(id);

            serialized.Should().BeOfType<string>().Which.Should().Be(value);
        }

        [Fact]
        public void Serialize_WithNull_ReturnsNull()
        {
            var idGraphType = new IdGraphType();

            var serialized = idGraphType.Serialize(null);

            serialized.Should().BeNull();
        }

        [Fact]
        public void ParseValue_WithValue_ReturnsId()
        {
            var idGraphType = new IdGraphType();
            var value = "BF389740-3EEE-4F2E-BBB1-1C4758B5CE11";

            var parsed = idGraphType.ParseValue(value);

            parsed.Should().BeOfType<Id>().Which.Value.Should().Be(value);
        }

        [Fact]
        public void ParseValue_WithNull_ReturnsNull()
        {
            var idGraphType = new IdGraphType();

            var parsed = idGraphType.ParseValue(null);

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseLiteral_WithStringValue_ReturnsId()
        {
            var idGraphType = new IdGraphType();
            var value = "BDCE0B55-8D23-47A4-9C66-C94F7D6D8C2A";

            var parsed = idGraphType.ParseLiteral(new StringValue(value));

            parsed.Should().BeOfType<Id>().Which.Value.Should().Be(value);
        }

        [Fact]
        public void ParseLiteral_WithIntValue_ReturnsId()
        {
            var idGraphType = new IdGraphType();
            var value = 43;

            var parsed = idGraphType.ParseLiteral(new IntValue(value));

            parsed.Should().BeOfType<Id>().Which.Value.Should().Be(value.ToString());
        }

        [Fact]
        public void ParseLiteral_WithNull_ReturnsNull()
        {
            var idGraphType = new IdGraphType();

            var parsed = idGraphType.ParseLiteral(null);

            parsed.Should().BeNull();
        }
    }
}
