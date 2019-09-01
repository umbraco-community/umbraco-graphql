using System;
using FluentAssertions;
using Our.Umbraco.GraphQL.Types;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types
{
    public class IdTests
    {
        [Fact]
        public void Ctor_WithNull_ThrowsArgumentNullException()
        {
            Action action = () => new Id(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_WithEmptyString_ThrowsArgumentException()
        {
            Action action = () => new Id(string.Empty);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WithValue_SetsValueProperty()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.Value.Should().Be(value);
        }

        [Fact]
        public void As_GuidWhenValueIsGuid_ReturnsGuid()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.As<Guid>().Should().Be(new Guid(value));
        }

        [Fact]
        public void As_IntWhenValueIsInt_ReturnsInt()
        {
            const string value = "1";

            var id = new Id(value);

            id.As<int>().Should().Be(1);
        }

        [Fact]
        public void As_IntWhenValueIsNotAnInt_ThrowsException()
        {
            const string value = "not an int";

            var id = new Id(value);

            Action action = () => id.As<int>();

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void ToString_ReturnsValue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.ToString().Should().Be(value);
        }

        [Fact]
        public void ImplicitCast_FromString_ReturnsNewId()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            Id id = value;

            id.ToString().Should().Be(value);
        }

        [Fact]
        public void ImplicitCast_FromNull_ThrowsArgumentNullException()
        {
            Action action = () =>
            {
                Id id = null;
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ImplicitCast_ToNullableIdFromString_ReturnsValue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            Id? id = value;

            id.Value.Value.Should().Be(value);
        }

        [Fact]
        public void ImplicitCast_IdWithValueToString_ReturnsValue()
        {
            var id = new Id("B894DD36-461D-4A94-845B-DB143573F214");

            string value = id;

            value.Should().Be(id.Value);
        }

        [Fact]
        public void ImplicitCast_NullIdToString_ReturnsNull()
        {
            string value = (Id?)null;

            value.Should().BeNull();
        }

        [Fact]
        public void Equals_WithSameId_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.Equals(id).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithIdWithSameValue_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);
            var other = new Id(value);

            id.Equals(other).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithIdWithIdAsObject_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);
            var other = new Id(value);

            id.Equals((object)other).Should().BeTrue();
        }

        [Fact]
        public void Equals_IdAndStringValue_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.Equals(value).Should().BeTrue();
        }

        [Fact]
        public void Equals_IdAndOtherValue_ReturnsFalse()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.Equals("other").Should().BeFalse();
        }

        [Fact]
        public void Equals_WithIdWithNull_ReturnsFalse()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.Equals((object)null).Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_WithSameId_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            (id == id).Should().BeTrue();
        }

        [Fact]
        public void EqualsOperator_WithIdWithSameValue_ReturnsTrue()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);
            var other = new Id(value);

            (id == other).Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator_WithSameId_ReturnsFalse()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            (id != id).Should().BeFalse();
        }

        [Fact]
        public void NotEqualsOperator_WithIdWithSameValue_ReturnsFalse()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);
            var other = new Id(value);

            (id != other).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ReturnsValuesHashCode()
        {
            const string value = "B894DD36-461D-4A94-845B-DB143573F214";

            var id = new Id(value);

            id.GetHashCode().Should().Be(value.GetHashCode());
        }
    }
}
