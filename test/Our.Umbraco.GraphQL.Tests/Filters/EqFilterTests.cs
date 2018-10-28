using Our.Umbraco.GraphQL.Filters;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Filters
{
    public class EqFilterTests : FilterTest
    {
        [Fact]
        public void IsSatisfiedBy_WithEqualStringValue_ReturnsTrue()
        {
            var value = "test";
            var filter = new EqFilter(ValueResolver, value);

            var result = filter.IsSatisfiedBy(value);

            Assert.True(result);
        }

        [Fact]
        public void IsSatisfiedBy_WithEqualIntValue_ReturnsTrue()
        {
            var value = 1;
            var filter = new EqFilter(ValueResolver, value);

            var result = filter.IsSatisfiedBy(value);

            Assert.True(result);
        }

        [Fact]
        public void IsSatisfiedBy_WithEqualObjectValue_ReturnsTrue()
        {
            var value = new object();
            var filter = new EqFilter(ValueResolver, value);

            var result = filter.IsSatisfiedBy(value);

            Assert.True(result);
        }

        [Fact]
        public void IsSatisfiedBy_WithNonEqualValue_ReturnsFalse()
        {
            var value = 1;
            var input = "test";
            var filter = new EqFilter(ValueResolver, value);

            var result = filter.IsSatisfiedBy(input);

            Assert.False(result);
        }
    }
}
