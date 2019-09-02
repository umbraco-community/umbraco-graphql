using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using GraphQL;
using GraphQL.Types;
using NSubstitute;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Resolvers;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types.Relay;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters
{
    public class GraphTypeAdapterTests
    {
        private GraphTypeAdapter CreateSUT(GraphVisitor visitor = null, ITypeRegistry typeRegistry = null) =>
            new GraphTypeAdapter(typeRegistry ?? new TypeRegistry(), new DefaultDependencyResolver(), visitor);

        [Fact]
        public void Adapt_Null_ThrowsArgumentNullException()
        {
            var adapter = CreateSUT();

            Action action = () => adapter.Adapt(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(AbstractClassWithoutDescription), typeof(InterfaceGraphType<AbstractClassWithoutDescription>))]
        [InlineData(typeof(ClassWithoutDescription), typeof(ObjectGraphType<ClassWithoutDescription>))]
        [InlineData(typeof(EnumWithoutDescription), typeof(NonNullGraphType<EnumerationGraphType<EnumWithoutDescription>>))]
        [InlineData(typeof(EnumWithoutDescription?), typeof(EnumerationGraphType<EnumWithoutDescription>))]
        [InlineData(typeof(IInterfaceWithoutDescription), typeof(InterfaceGraphType<IInterfaceWithoutDescription>))]
        public void Adapt_Object_ReturnsGraphType(Type type, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.Should().BeOfType(expectedType);
        }

        [Theory]
        [InlineData(typeof(AbstractClassWithName), "MyAbstractClass")]
        [InlineData(typeof(ClassWithName), "MyClass")]
        [InlineData(typeof(EnumWithName?), "MyEnum")]
        [InlineData(typeof(IInterfaceWithName), "MyInterface")]
        public void Adapt_ObjectWithNameAttribute_SetsNameFromAttribute(Type type, string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData(typeof(AbstractClassWithoutDescription), nameof(AbstractClassWithoutDescription))]
        [InlineData(typeof(ClassWithoutDescription), nameof(ClassWithoutDescription))]
        [InlineData(typeof(EnumWithoutDescription?), nameof(EnumWithoutDescription))]
        [InlineData(typeof(IInterfaceWithoutDescription), nameof(IInterfaceWithoutDescription))]
        public void Adapt_Object_SetsNameToTypeName(Type type, string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Adapt_NullableType_SetsName()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(EnumWithDescription?).GetTypeInfo());

            graphType.Name.Should().Be("EnumWithDescription");
        }

        [Theory]
        [InlineData(typeof(AbstractClassWithDescription), "Abstract class description.")]
        [InlineData(typeof(ClassWithDescription), "Class description.")]
        [InlineData(typeof(EnumWithDescription?), "Enum description.")]
        [InlineData(typeof(IInterfaceWithDescription), "Interface description.")]
        public void Adapt_ObjectWithDescriptionAttribute_SetsDescriptionFromAttribute(Type type, string expectedDescription)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.Description.Should().Be(expectedDescription);
        }

        [Theory]
        [InlineData(typeof(AbstractClassWithDeprecationReason), "Abstract class deprecation reason.")]
        [InlineData(typeof(ClassWithDeprecationReason), "Class deprecation reason.")]
        [InlineData(typeof(EnumWithDeprecationReason?), "Enum deprecation reason.")]
        [InlineData(typeof(IInterfaceWithDeprecationReason), "Interface deprecation reason.")]
        public void Adapt_ObjectWithDeprecatedAttribute_SetsDeprecationReasonFromAttribute(Type type, string expectedDeprecationReason)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.DeprecationReason.Should().Be(expectedDeprecationReason);
        }

        [Fact]
        public void Adapt_Object_TypeInfoIsAddedToMetadata()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Metadata.Should().ContainKey(nameof(TypeInfo)).WhichValue.Should()
                .Be(typeof(ClassWithoutDescription));
        }

        [Fact]
        public void Adapt_Enum_AddsEnumValues()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<EnumWithoutDescription?>();

            graphType.Should().BeAssignableTo<EnumerationGraphType>()
                .Which.Values.Should().Contain(x => x.Name == "ONE")
                .And.Contain(x => x.Name == "SEVEN")
                .And.Contain(x => x.Name == "FORTY_TWO");
        }

        [Theory]
        [InlineData("One")]
        [InlineData("Seven")]
        [InlineData("FortyTwo")]
        public void Adapt_EnumValueWithNameAttribute_SetsEnumFieldNameFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<EnumWithName?>();

            graphType.Should().BeAssignableTo<EnumerationGraphType>()
                .Which.Values.Should().Contain(x => x.Name == expectedName);
        }

        [Theory]
        [InlineData("One description.")]
        [InlineData("Seven description.")]
        [InlineData("FortyTwo description.")]
        public void Adapt_EnumValueWithDescriptionAttribute_SetsEnumFieldDescriptionFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<EnumWithDescription?>();

            graphType.Should().BeAssignableTo<EnumerationGraphType>()
                .Which.Values.Should().Contain(x => x.Description == expectedName);
        }

        [Theory]
        [InlineData("One deprecation reason.")]
        [InlineData("Seven deprecation reason.")]
        [InlineData("FortyTwo deprecation reason.")]
        public void Adapt_EnumValueWithDeprecatedAttribute_SetsEnumFieldDeprecationReasonFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<EnumWithDeprecationReason?>();

            graphType.Should().BeAssignableTo<EnumerationGraphType>()
                .Which.Values.Should().Contain(x => x.DeprecationReason == expectedName);
        }

        [Fact]
        public void Adapt_PublicFields_AddsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(ClassWithoutDescription.PublicField));
        }

        [Fact]
        public void Adapt_InheritedPublicFields_AddsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<InheritedClass>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(InheritedClass.PublicField));
        }

        [Fact]
        public void Adapt_PrivateFields_DoesNotAddFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == "_privateField");
        }

        [Fact]
        public void Adapt_ObjectFields_DoesNotAddFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.ObjectField));
        }

        [Fact]
        public void Adapt_StaticFields_DoesNotAddFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.StaticField));
        }

        [Fact]
        public void Adapt_PublicProperties_AddsAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(ClassWithoutDescription.PublicProperty));
        }

        [Fact]
        public void Adapt_PrivateProperties_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == "PrivateProperty");
        }

        [Fact]
        public void Adapt_PrivateGetProperties_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.PrivateGetProperty));
        }

        [Fact]
        public void Adapt_SetOnlyProperties_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.SetOnlyProperty));
        }

        [Fact]
        public void Adapt_StaticProperties_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.StaticProperty));
        }

        [Fact]
        public void Adapt_ObjectProperties_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.ObjectProperty));
        }

        [Fact]
        public void Adapt_PublicMethods_AddsAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(ClassWithoutDescription.PublicMethod));
        }

        [Fact]
        public void Adapt_PrivateMethods_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == "PrivateMethod");
        }

        [Fact]
        public void Adapt_StaticMethods_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.StaticMethod));
        }

        [Fact]
        public void Adapt_MethodSpecialNames_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == $"get_{nameof(ClassWithoutDescription.PublicProperty)}");
        }

        [Fact]
        public void Adapt_MethodsVoidReturnType_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.VoidMethod));
        }

        [Fact]
        public void Adapt_MethodsObjectReturnType_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == nameof(ClassWithoutDescription.ObjectMethod));
        }

        [Fact]
        public void Adapt_MethodsDefinedOnObject_DoesNotAddAsFields()
        {
            var adapter = CreateSUT();

            var graphType = (IObjectGraphType) adapter.Adapt<ClassWithOverriddenFields>();

            graphType.Fields.Should().NotContain(x => x.Name == nameof(ClassWithOverriddenFields.ToString));
        }

        [Fact]
        public void Adapt_MemberType_IsAddedToMetadata()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.PublicField))
                .Which.Metadata.Should().ContainKey(nameof(MemberInfo))
                .WhichValue.Should()
                .Be(typeof(ClassWithoutDescription).GetField(nameof(ClassWithoutDescription.PublicField)));
        }

        [Theory]
        [InlineData("MyField")]
        [InlineData("MyProperty")]
        [InlineData("MyMethod")]
        public void Adapt_FieldWithNameAttribute_SetsFieldNameFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithName>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == expectedName);
        }

        [Theory]
        [InlineData("Field description.")]
        [InlineData("Property description.")]
        [InlineData("Method description.")]
        public void Adapt_FieldWithDescriptionAttribute_SetsFieldDescriptionFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Description == expectedName);
        }

        [Theory]
        [InlineData("Field deprecation reason.")]
        [InlineData("Property deprecation reason.")]
        [InlineData("Method deprecation reason.")]
        public void Adapt_FieldWithDeprecatedAttribute_SetsFieldDeprecationReasonFromAttribute(string expectedName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithDeprecationReason>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.DeprecationReason == expectedName);
        }

        [Theory]
        [InlineData(nameof(AbstractClassWithoutDescription.PublicField))]
        [InlineData(nameof(AbstractClassWithoutDescription.PublicProperty))]
        [InlineData(nameof(AbstractClassWithoutDescription.PublicMethod))]
        public void Adapt_AbstractClass_MembersAreAddedAsFields(string memberName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<AbstractClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IInterfaceGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == memberName);
        }

        [Theory]
        [InlineData(nameof(IInterfaceWithoutDescription.Property))]
        [InlineData(nameof(IInterfaceWithoutDescription.Method))]
        public void Adapt_Interface_MembersAreAddedAsFields(string memberName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<IInterfaceWithoutDescription>();

            graphType.Should().BeAssignableTo<IInterfaceGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == memberName);
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.IgnoredField))]
        [InlineData(nameof(ClassWithoutDescription.IgnoredMethod))]
        [InlineData(nameof(ClassWithoutDescription.IgnoredProperty))]
        public void Adapt_MembersWithIgnoreAttribute_MembersAreNotAddedAsFields(string memberName)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().NotContain(x => x.Name == memberName);
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.EnumWithoutDescription), typeof(NonNullGraphType<EnumerationGraphType<EnumWithoutDescription>>))]
        [InlineData(nameof(ClassWithoutDescription.NullableEnumWithoutDescription), typeof(EnumerationGraphType<EnumWithoutDescription>))]
        [InlineData(nameof(ClassWithoutDescription.ListOfClassWithoutDescription), typeof(ListGraphType<ObjectGraphType<ClassWithoutDescription>>))]
        public void Adapt_Members_SetsCorrectResolvedType(string memberName, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == memberName)
                .Which.ResolvedType.Should().BeOfType(expectedType);
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.PublicField), typeof(NonNullGraphType<IntGraphType>))]
        [InlineData(nameof(ClassWithoutDescription.PublicProperty), typeof(StringGraphType))]
        [InlineData(nameof(ClassWithoutDescription.PublicMethod), typeof(NonNullGraphType<BooleanGraphType>))]
        [InlineData(nameof(ClassWithoutDescription.ListOfString), typeof(ListGraphType<StringGraphType>))]
        [InlineData(nameof(ClassWithoutDescription.ListOfInt), typeof(ListGraphType<NonNullGraphType<IntGraphType>>))]
        public void Adapt_Members_SetsCorrectType(string memberName, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt<ClassWithoutDescription>();

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == memberName)
                .Which.Type.Should().Be(expectedType);
        }

        [Theory]
        [InlineData(typeof(string), typeof(StringGraphType))]
        [InlineData(typeof(int?), typeof(IntGraphType))]
        [InlineData(typeof(bool?), typeof(BooleanGraphType))]
        [InlineData(typeof(DateTime?), typeof(DateTimeGraphType))]
        [InlineData(typeof(int), typeof(NonNullGraphType<IntGraphType>))]
        [InlineData(typeof(bool), typeof(NonNullGraphType<BooleanGraphType>))]
        [InlineData(typeof(DateTime), typeof(NonNullGraphType<DateTimeGraphType>))]
        public void Adapt_WithNullableScalarType_ResolvesType(Type type, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(type.GetTypeInfo());

            graphType.Should().BeOfType(expectedType);
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.NonNullField), typeof(NonNullGraphType<StringGraphType>))]
        [InlineData(nameof(ClassWithoutDescription.NonNullMethod), typeof(NonNullGraphType<StringGraphType>))]
        [InlineData(nameof(ClassWithoutDescription.NonNullProperty), typeof(NonNullGraphType<StringGraphType>))]
        public void Adapt_MembersWithNonNullAttribute_IsNonNullType(string memberName, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == memberName)
                .Which.Type.Should().Be(expectedType);
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.NonNullItemField), typeof(ListGraphType<NonNullGraphType<StringGraphType>>))]
        [InlineData(nameof(ClassWithoutDescription.NonNullItemMethod), typeof(ListGraphType<NonNullGraphType<StringGraphType>>))]
        [InlineData(nameof(ClassWithoutDescription.NonNullItemProperty), typeof(ListGraphType<NonNullGraphType<StringGraphType>>))]
        public void Adapt_EnumerableMembersWithNonNullItemAttribute_TypeIsNonNullType(string memberName, Type expectedType)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == memberName)
                .Which.Type.Should().Be(expectedType);
        }

        [Fact]
        public void Adapt_MembersWithNonNullAttribute_IsNonNullResolvedType()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.NonNullOfComplexTypeField))
                .Which.ResolvedType.Should().BeAssignableTo<NonNullGraphType>();
        }

        [Fact]
        public void Adapt_EnumerableMembersWithNonNullItemAttribute_ResolvedTypeIsNonNullType()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.NonNullItemOfComplexTypeField))
                .Which.ResolvedType.Should().BeAssignableTo<ListGraphType>()
                .Which.ResolvedType.Should().BeAssignableTo<NonNullGraphType>();
        }

        [Theory]
        [InlineData(nameof(ClassWithoutDescription.FieldWithDefaultValue), "FieldDefaultValue")]
        [InlineData(nameof(ClassWithoutDescription.MethodWithDefaultValue), "MethodDefaultValue")]
        [InlineData(nameof(ClassWithoutDescription.PropertyWithDefaultValue), 42)]
        public void Adapt_MembersWithDefaultValueAttribute_FieldsDefaultValueIsSet(string memberName,
            object expectedDefaultValue)
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == memberName)
                .Which.DefaultValue.Should().Be(expectedDefaultValue);
        }

        [Fact]
        public void Adapt_MethodsWithArguments_IsAddedAsArguments()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithArguments))
                .Which.Arguments.Should().HaveCount(2)
                .And.ContainSingle(x => x.Name == "name")
                .Which.ResolvedType.Should().BeAssignableTo<NonNullGraphType<StringGraphType>>();
        }

        [Fact]
        public void Adapt_MethodsWithValueTypeArguments_IsAddedAsArguments()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithArguments))
                .Which.Arguments.Should().HaveCount(2)
                .And.ContainSingle(x => x.Name == "year")
                .Which.ResolvedType.Should().BeAssignableTo<NonNullGraphType<IntGraphType>>();
        }

        [Fact]
        public void Adapt_MethodArgumentWithDefaultValue_ArgumentShouldNotBeNonNull()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithArgumentDefaultValue))
                .Which.Arguments.Should().HaveCount(2)
                .And.ContainSingle(x => x.Name == "year")
                .Which.ResolvedType.Should().NotBeAssignableTo<NonNullGraphType>();
        }

        [Fact]
        public void Adapt_MethodArgumentWithDefaultValue_ArgumentDefaultValueIsSet()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithArgumentDefaultValue))
                .Which.Arguments.Should().HaveCount(2)
                .And.ContainSingle(x => x.Name == "year")
                .Which.DefaultValue.Should().Be(1970);
        }

        [Fact]
        public void Adapt_MethodArgumentWithDefaultValueAttribute_ArgumentDefaultValueIsSetToAttributeValue()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithArgumentDefaultValue))
                .Which.Arguments.Should().HaveCount(2)
                .And.ContainSingle(x => x.Name == "name")
                .Which.DefaultValue.Should().Be("john");
        }

        [Fact]
        public void Adapt_MethodArgumentWithNameAttribute_ArgumentNameIsSetToAttributeValue()
        {
            var adapter = CreateSUT();

            var graphType = (IComplexGraphType) adapter.Adapt(typeof(ClassWithName).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithName.MethodWithArgument))
                .Which.Arguments.Should().ContainSingle(x => x.Name == "myName");
        }

        [Fact]
        public void Adapt_MethodArgumentWithDescriptionAttribute_ArgumentDescriptionIsSetToAttributeValue()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithDescription.MethodWithArgument))
                .Which.Arguments.Should().ContainSingle(x => x.Name == "name")
                .Which.Description.Should().Be("Argument description.");
        }

        [Fact]
        public void Adapt_MethodWithComplexArgument_ArgumentIsInputObjectGraphType()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithComplexArgument))
                .Which.Arguments.Should().ContainSingle(x => x.Name == "filter")
                .Which.ResolvedType.Should().BeAssignableTo<NonNullGraphType>()
                .Which.ResolvedType.Should().BeAssignableTo<IInputObjectGraphType>();
        }

        [Fact]
        public void Adapt_MethodWithEnumArgument_ArgumentIsEnumerationGraphType()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithEnumArgument))
                .Which.Arguments.Should().ContainSingle(x => x.Name == "enumWithName")
                .Which.ResolvedType.Should().BeAssignableTo<EnumerationGraphType>();
        }

        [Fact]
        public void Adapt_MethodArgumentWithInjectAttribute_ArgumentIsNotAdded()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.MethodWithInjectedArgument))
                .Which.Arguments.Should().NotContain(x => x.Name == "injected");
        }

        [Fact]
        public void Adapt_FieldResolver_IsFieldResolver()
        {
            var adapter = CreateSUT();

            var graphType = adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            graphType.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().ContainSingle(x => x.Name == nameof(ClassWithoutDescription.PublicField))
                .Which.Resolver.Should().BeOfType<FieldResolver>();
        }

        [Fact]
        public void Adapt_ClassWithVisitor_CallsVisitWithGraphType()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var adapter = CreateSUT(visitor);

            var graphType = (IObjectGraphType) adapter.Adapt(typeof(ClassWithDescription).GetTypeInfo());

            visitor.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Adapt_AbstractClassWithVisitor_CallsVisitWithGraphType()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var adapter = CreateSUT(visitor);

            var graphType = (IInterfaceGraphType) adapter.Adapt(typeof(AbstractClassWithoutDescription).GetTypeInfo());

            visitor.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Adapt_InterfaceWithVisitor_CallsVisitWithGraphType()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var adapter = CreateSUT(visitor);

            var graphType = (IInterfaceGraphType) adapter.Adapt(typeof(IInterfaceWithoutDescription).GetTypeInfo());

            visitor.Received(1).Visit(Arg.Is(graphType));
        }

        [Fact]
        public void Adapt_InputTypeWithVisitor_CallsVisitWithGraphType()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var adapter = CreateSUT(visitor);

            var graphType = (IObjectGraphType) adapter.Adapt(typeof(ClassWithoutDescription).GetTypeInfo());

            var filterFieldGraphType = graphType.Fields.Single(x => x.Name == nameof(ClassWithoutDescription.MethodWithComplexArgument))
                .Arguments.Single(x => x.Name == "filter").ResolvedType;

            var inputObjectGraphType = filterFieldGraphType.Should().BeAssignableTo<NonNullGraphType>()
                .Which.ResolvedType.Should().BeAssignableTo<IInputObjectGraphType>()
                .Subject;

            visitor.Received(1).Visit(Arg.Is(inputObjectGraphType));
        }

        [Fact]
        public void Adapt_WithExtendTypes_AddsFieldsFromExtendTypes()
        {
            var typeRegistry = new TypeRegistry();
            typeRegistry.Extend<InheritedClass, ClassWithDescription>();

            var adapter = CreateSUT(typeRegistry: typeRegistry);

            var graphTypeDefinition = adapter.Adapt(typeof(InheritedClass).GetTypeInfo());

            graphTypeDefinition.Should().BeAssignableTo<IComplexGraphType>()
                .Which.Fields.Should().Contain(field => field.Name == nameof(ClassWithDescription.FieldWithDescription))
                .And.Contain(field => field.Name == nameof(ClassWithDescription.MethodWithArgument))
                .And.Contain(field => field.Name == nameof(ClassWithDescription.MethodWithDescription))
                .And.Contain(field => field.Name == nameof(ClassWithDescription.PropertyWithDescription));
        }

        [Fact]
        public void Adapt_WithExtendTypes_RecursiveAddsFieldsFromExtendTypes()
        {
            var typeRegistry = new TypeRegistry();
            typeRegistry.Extend<ClassWithName, InheritedClass>();

            var builder = CreateSUT(typeRegistry: typeRegistry);

            var graphTypeDefinition = builder.Adapt(typeof(ClassWithName).GetTypeInfo());

            graphTypeDefinition.Should().BeAssignableTo<IComplexGraphType>()
                .Which.Fields.Should().Contain(field => field.Name == nameof(InheritedClass.PublicField))
                .And.Contain(field => field.Name == nameof(InheritedClass.PublicMethod))
                .And.Contain(field => field.Name == nameof(InheritedClass.PublicProperty));
        }

        [Fact]
        [Fact]
        public void Adapt_ConnectionMember_ResolvesToConnectionGraphType()
        {
            var builder = CreateSUT();

            var graphTypeDefinition = builder.Adapt(typeof(ClassWithName).GetTypeInfo());

            graphTypeDefinition.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(ClassWithName.Descriptions))
                .Which.ResolvedType.Should().BeOfType<ConnectionGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == "edges")
                .Which.ResolvedType.Should().BeOfType<ListGraphType>()
                .Which.ResolvedType.Should().BeOfType<EdgeGraphType>();
        }

        [Fact]
        public void Adapt_ConnectionMemberWithRegisteredType_ResolvesToConnectionGraphType()
        {
            var typeRegistry = new TypeRegistry();
            typeRegistry.Add<ClassWithDescription, ObjectGraphType>();
            var builder = CreateSUT(typeRegistry: typeRegistry);

            var graphTypeDefinition = builder.Adapt(typeof(ClassWithName).GetTypeInfo());

            graphTypeDefinition.Should().BeAssignableTo<IObjectGraphType>()
                .Which.Fields.Should().Contain(x => x.Name == nameof(ClassWithName.Descriptions))
                .Which.Type.Should().BeAssignableTo<ConnectionGraphType<ObjectGraphType>>();
        }

        private abstract class AbstractClassWithoutDescription
        {
            public string PublicField;
            public string PublicProperty { get; set; }
            public string PublicMethod() => null;
        }

        private class ClassWithoutDescription
        {
            private string _privateField;
            public int PublicField;
            public object ObjectField;
            public static string StaticField;
            public static string StaticProperty { get; set; }
            public static string StaticMethod() => null;
            public bool PublicMethod() => false;
            private string PrivateMethod() => null;
            public string PrivateGetProperty { private get; set; }

            public string SetOnlyProperty
            {
                set => _privateField = value;
            }

            public object ObjectMethod() => null;
            public void VoidMethod(){}
            public string PublicProperty { get; set; }
            public object ObjectProperty { get; set; }

            [Ignore] public string IgnoredField;

            [Ignore]
            public string IgnoredProperty { get; set; }

            [Ignore]
            public string IgnoredMethod() => null;

            public EnumWithoutDescription EnumWithoutDescription;
            public EnumWithoutDescription? NullableEnumWithoutDescription;

            public IEnumerable<string> ListOfString { get; set; }
            public IEnumerable<int> ListOfInt { get; set; }
            public IEnumerable<ClassWithoutDescription> ListOfClassWithoutDescription { get; set; }

            [NonNull]
            public string NonNullField;

            [NonNull]
            public ClassWithoutDescription NonNullOfComplexTypeField;

            [NonNull]
            public string NonNullProperty { get; set; }

            [NonNull]
            public string NonNullMethod() => null;

            [NonNullItem]
            public IEnumerable<string> NonNullItemField;

            [NonNullItem]
            public IEnumerable<string> NonNullItemProperty { get; set; }

            [NonNullItem]
            public IEnumerable<string> NonNullItemMethod() => null;

            [NonNullItem]
            public IEnumerable<ClassWithName> NonNullItemOfComplexTypeField;

            [DefaultValue("FieldDefaultValue")]
            public string FieldWithDefaultValue;

            [DefaultValue(42)]
            public string PropertyWithDefaultValue { get; set; }

            [DefaultValue("MethodDefaultValue")]
            public string MethodWithDefaultValue() => null;

            public string MethodWithArguments(string name, int year) => null;
            public string MethodWithArgumentDefaultValue([DefaultValue("john")] string name, int year = 1970) => null;
            public string MethodWithComplexArgument(Filter filter) => null;
            public string MethodWithEnumArgument(EnumWithName? enumWithName) => null;
            public string MethodWithInjectedArgument([Inject]Filter injected) => null;
        }

        private enum EnumWithoutDescription
        {
            One,
            Seven,
            FortyTwo
        }

        private class InheritedClass : AbstractClassWithoutDescription
        {
        }

        private interface IInterfaceWithoutDescription
        {
            string Property { get; set; }
            string Method();
        }

        [Name("MyAbstractClass")]
        private abstract class AbstractClassWithName
        {
        }

        [Name("MyClass")]
        private class ClassWithName
        {
            [Name("MyField")]
            public string FieldWithName;
            [Name("MyProperty")]
            public string PropertyWithName { get; set; }
            [Name("MyMethod")]
            public string MethodWithName() => null;

            public string MethodWithArgument([Name("myName")] string name) => null;

            public Connection<ClassWithDescription> Descriptions() => null;
        }

        [Name("MyEnum")]
        private enum EnumWithName
        {
            [Name("One")]
            One,
            [Name("Seven")]
            Seven,
            [Name("FortyTwo")]
            FortyTwo
        }

        [Name("MyInterface")]
        private interface IInterfaceWithName
        {
        }

        [Attributes.Description("Abstract class description.")]
        private abstract class AbstractClassWithDescription
        {
        }

        [Attributes.Description("Class description.")]
        private class ClassWithDescription
        {
            [Attributes.Description("Field description.")]
            public string FieldWithDescription;

            [Attributes.Description("Property description.")]
            public string PropertyWithDescription { get; set; }
            [Attributes.Description("Method description.")]
            public string MethodWithDescription() => null;

            public string MethodWithArgument([Description("Argument description.")] string name) => null;
        }

        [Attributes.Description("Enum description.")]
        private enum EnumWithDescription
        {
            [Attributes.Description("One description.")]
            One,
            [Attributes.Description("Seven description.")]
            Seven,
            [Attributes.Description("FortyTwo description.")]
            FortyTwo
        }

        [Attributes.Description("Interface description.")]
        private interface IInterfaceWithDescription
        {
        }

        [Deprecated("Abstract class deprecation reason.")]
        private abstract class AbstractClassWithDeprecationReason
        {
        }

        [Deprecated("Class deprecation reason.")]
        private class ClassWithDeprecationReason
        {
            [Deprecated("Field deprecation reason.")]
            public string FieldWithDescription;
            [Deprecated("Property deprecation reason.")]
            public string PropertyWithDescription { get; set; }
            [Deprecated("Method deprecation reason.")]
            public string MethodWithDescription() => null;
        }

        [Deprecated("Enum deprecation reason.")]
        private enum EnumWithDeprecationReason
        {
            [Deprecated("One deprecation reason.")]
            One,
            [Deprecated("Seven deprecation reason.")]
            Seven,
            [Deprecated("FortyTwo deprecation reason.")]
            FortyTwo
        }

        [Deprecated("Interface deprecation reason.")]
        private interface IInterfaceWithDeprecationReason
        {
        }

        private class Filter
        {
            public string Name { get; set; }
        }

        private class ClassWithOverriddenFields
        {
            public override bool Equals(object obj) => false;
            public override string ToString() => null;
            public override int GetHashCode() => 0;
        }
    }
}
