using System;
using System.Reflection;
using FluentAssertions;
using GraphQL.Language.AST;
using Our.Umbraco.GraphQL.Adapters.Types;
using Umbraco.Core;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types
{
    public class UdiGraphTypeTests
    {
        public UdiGraphTypeTests()
        {
            // we need to set _scanned to true to avoid Udi to try scanning for types
            // which uses the static `Current` service locator.
            typeof(Udi).GetField("_scanned", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, true);
        }

        [Fact]
        public void Ctor_SetsName()
        {
            var udiGraphType = new UdiGraphType();

            udiGraphType.Name.Should().Be("UDI");
        }

        [Fact]
        public void Ctor_SetsDescription()
        {
            var udiGraphType = new UdiGraphType();

            udiGraphType.Description.Should().Be("Represents an entity identifier.");
        }

        [Fact]
        public void Serialize_WithUdi_ReturnsValueToString()
        {
            var udiGraphType = new UdiGraphType();
            var value = new Guid("{A2A6F423-7C73-4B82-A86B-D8078F81E258}");
            var udi = new GuidUdi("document", value);

            var serialized = udiGraphType.Serialize(udi);

            serialized.Should().BeOfType<string>().Which.Should().Be(udi.ToString());
        }

        [Fact]
        public void Serialize_WithNull_ReturnsNull()
        {
            var udiGraphType = new UdiGraphType();

            var serialized = udiGraphType.Serialize(null);

            serialized.Should().BeNull();
        }

        [Fact]
        public void ParseValue_WithUdiValue_ReturnsUdi()
        {
            var udiGraphType = new UdiGraphType();
            var value = "umb://document/c0126770c4dd4c80b48464c5f38658c1";

            var parsed = udiGraphType.ParseValue(value);

            parsed.Should().BeAssignableTo<Udi>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseValue_WithNonUdiValue_ReturnsNull()
        {
            var udiGraphType = new UdiGraphType();
            var value = "not an udi";

            var parsed = udiGraphType.ParseValue(value);

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseValue_WithNull_ReturnsNull()
        {
            var udiGraphType = new UdiGraphType();

            var parsed = udiGraphType.ParseValue(null);

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseLiteral_WithUdiValue_ReturnsUdi()
        {
            var udiGraphType = new UdiGraphType();
            var value = "umb://document/c0126770c4dd4c80b48464c5f38658c1";

            var parsed = udiGraphType.ParseLiteral(new StringValue(value));

            parsed.Should().BeAssignableTo<Udi>().Which.ToString().Should().Be(value);
        }

        [Fact]
        public void ParseLiteral_WithNonUdiValue_ReturnsNull()
        {
            var udiGraphType = new UdiGraphType();
            var value = 4;

            var parsed = udiGraphType.ParseLiteral(new IntValue(value));

            parsed.Should().BeNull();
        }

        [Fact]
        public void ParseLiteral_WithNull_ReturnsNull()
        {
            var udiGraphType = new UdiGraphType();

            var parsed = udiGraphType.ParseLiteral(null);

            parsed.Should().BeNull();
        }
    }
}
