using System;
using System.IO;
using FluentAssertions;
using GraphQL;
using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Json.Converters;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Json
{
    public class InputConverterTests
    {
        [Fact]
        public void CanConvert_WithInputsType_ReturnsTrue()
        {
            var converter = new InputsConverter();

            converter.CanConvert(typeof(Inputs))
                .Should().BeTrue();
        }

        [Fact]
        public void CanConvert_WithNonInputsType_ReturnsFalse()
        {
            var converter = new InputsConverter();

            converter.CanConvert(typeof(string))
                .Should().BeFalse();
        }

        [Fact]
        public void CanWrite_ReturnsFalse()
        {
            var converter = new InputsConverter();

            converter.CanWrite.Should().BeFalse();
        }

        [Fact]
        public void WriteJson_ThrowsNotSupportedException()
        {
            var converter = new InputsConverter();

            Action action = () => converter.WriteJson(null, null, null);
            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ReadJson_WithData_ReturnsInputs()
        {
            var converter = new InputsConverter();

            var data = converter.ReadJson(new JsonTextReader(new StringReader("{\"name\": \"test\"}")), typeof(Inputs),
                null, new JsonSerializer());

            data.Should().BeOfType<Inputs>()
                .Which.Should().ContainKey("name")
                .WhichValue.Should().Be("test");
        }

        [Fact]
        public void ReadJson_WithNoData_ReturnsEmptyInputs()
        {
            var converter = new InputsConverter();

            var data = converter.ReadJson(new JsonTextReader(new StringReader("null")), typeof(Inputs),
                null, new JsonSerializer());

            data.Should().BeOfType<Inputs>()
                .Which.Should().BeEmpty();
        }
    }
}
