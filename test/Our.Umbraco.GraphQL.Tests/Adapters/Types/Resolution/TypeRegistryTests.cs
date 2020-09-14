using System;
using System.Reflection;
using System.Web;
using FluentAssertions;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.Relay;
using Xunit;
using GuidGraphType = Our.Umbraco.GraphQL.Adapters.Types.GuidGraphType;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Types.Resolution
{
    public class TypeRegistryTests
    {
        private TypeRegistry CreateSUT() => new TypeRegistry();

        [Fact]
        public void Add_WithTypes_DoesNotThrow()
        {
            var typeRegistry = CreateSUT();

            Action action = () => typeRegistry.Add<MyType, MyGraphType>();

            action.Should().NotThrow();
        }

        [Fact]
        public void Add_TypeAlreadyAdded_Throws()
        {
            var typeRegistry = CreateSUT();

            Action action = () => typeRegistry.Add<string, StringGraphType>();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Get_WithAddedType_ReturnsType()
        {
            var typeRegistry = CreateSUT();
            typeRegistry.Add<MyType, MyGraphType>();

            var result = typeRegistry.Get(typeof(MyType).GetTypeInfo());

            result.Should().Be<MyGraphType>();
        }

        [Fact]
        public void Get_TypeIsNotAdded_ReturnsNull()
        {
            var typeRegistry = CreateSUT();

            var result = typeRegistry.Get(typeof(MyType).GetTypeInfo());

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(string), typeof(StringGraphType))]
        [InlineData(typeof(byte), typeof(IntGraphType))]
        [InlineData(typeof(short), typeof(IntGraphType))]
        [InlineData(typeof(ushort), typeof(IntGraphType))]
        [InlineData(typeof(int), typeof(IntGraphType))]
        [InlineData(typeof(uint), typeof(IntGraphType))]
        [InlineData(typeof(long), typeof(IntGraphType))]
        [InlineData(typeof(ulong), typeof(IntGraphType))]
        [InlineData(typeof(decimal), typeof(DecimalGraphType))]
        [InlineData(typeof(double), typeof(FloatGraphType))]
        [InlineData(typeof(float), typeof(FloatGraphType))]
        [InlineData(typeof(bool), typeof(BooleanGraphType))]
        [InlineData(typeof(Guid), typeof(GuidGraphType))]
        [InlineData(typeof(DateTime), typeof(DateTimeGraphType))]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetGraphType))]
        [InlineData(typeof(TimeSpan), typeof(TimeSpanMillisecondsGraphType))]
        [InlineData(typeof(IHtmlString), typeof(HtmlGraphType))]
        [InlineData(typeof(Uri), typeof(UriGraphType))]
        [InlineData(typeof(Id), typeof(IdGraphType))]
        [InlineData(typeof(PageInfo), typeof(PageInfoGraphType))]
        [InlineData(typeof(JToken), typeof(JsonGraphType))]
        [InlineData(typeof(JObject), typeof(JsonGraphType))]
        [InlineData(typeof(JArray), typeof(JsonGraphType))]
        public void Get_Type_ReturnsRegisteredType(Type type, Type graphType)
        {
            var typeRegistry = CreateSUT();

            typeRegistry.Get(type.GetTypeInfo()).Should().Be(graphType);
        }

        [Theory]
        [InlineData(typeof(byte?), typeof(IntGraphType))]
        [InlineData(typeof(short?), typeof(IntGraphType))]
        [InlineData(typeof(ushort?), typeof(IntGraphType))]
        [InlineData(typeof(int?), typeof(IntGraphType))]
        [InlineData(typeof(uint?), typeof(IntGraphType))]
        [InlineData(typeof(long?), typeof(IntGraphType))]
        [InlineData(typeof(ulong?), typeof(IntGraphType))]
        [InlineData(typeof(decimal?), typeof(DecimalGraphType))]
        [InlineData(typeof(double?), typeof(FloatGraphType))]
        [InlineData(typeof(float?), typeof(FloatGraphType))]
        [InlineData(typeof(bool?), typeof(BooleanGraphType))]
        [InlineData(typeof(Guid?), typeof(GuidGraphType))]
        [InlineData(typeof(DateTime?), typeof(DateTimeGraphType))]
        [InlineData(typeof(DateTimeOffset?), typeof(DateTimeOffsetGraphType))]
        [InlineData(typeof(TimeSpan?), typeof(TimeSpanMillisecondsGraphType))]
        [InlineData(typeof(Id?), typeof(IdGraphType))]
        public void Get_NullableType_ReturnsRegisteredType(Type type, Type graphType)
        {
            var typeRegistry = CreateSUT();

            typeRegistry.Get(type.GetTypeInfo()).Should().Be(graphType);
        }
        [Fact]
        public void AddExtend_WithTypes_DoesNotThrow()
        {
            var typeRegistry = CreateSUT();

            Action action = () => typeRegistry.Extend<TypeToExtend, MyType>();

            action.Should().NotThrow();
        }

        [Fact]
        public void GetExtends_WithAddedType_ShouldContainType()
        {
            var typeRegistry = CreateSUT();
            typeRegistry.Extend<TypeToExtend, MyType>();

            var result = typeRegistry.GetExtending(typeof(TypeToExtend).GetTypeInfo());

            result.Should().Contain(typeof(MyType).GetTypeInfo());
        }

        [Fact]
        public void GetExtends_WithMultipleTypesAdded_ShouldContainTypes()
        {
            var typeRegistry = CreateSUT();
            typeRegistry.Extend<TypeToExtend, MyType>();
            typeRegistry.Extend<TypeToExtend, MyType2>();

            var result = typeRegistry.GetExtending(typeof(TypeToExtend).GetTypeInfo());

            result.Should().Contain(typeof(MyType).GetTypeInfo())
                .And.Contain(typeof(MyType2).GetTypeInfo());
        }


        [Fact]
        public void GetExtends_NoTypeAdded_ReturnsEmptyEnumerable()
        {
            var typeRegistry = CreateSUT();

            var result = typeRegistry.GetExtending(typeof(TypeToExtend).GetTypeInfo());

            result.Should().BeEmpty();
        }

        private class MyType {}
        private class MyGraphType : ObjectGraphType<MyType>{}
        private class MyType2 {}
        private class TypeToExtend {}
    }
}
