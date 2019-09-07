using FluentAssertions;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Xunit;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class PublishedElementInterfaceGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var graphType = new PublishedElementInterfaceGraphType();

            graphType.Name.Should().Be("PublishedElement");
        }

        [Fact]
        public void Ctor_AddsFields()
        {
            var graphType = new PublishedElementInterfaceGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "_contentType")
                .And.Contain(x => x.Name == "_id");
        }

        [Fact]
        public void ContentTypeField_Type_ShouldBePublishedContentGraphType()
        {
            var graphType = new PublishedElementInterfaceGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "_contentType")
                .Which.Type.Should().Be<NonNullGraphType<PublishedContentTypeGraphType>>();
        }

        [Fact]
        public void IdField_Type_ShouldBeIdGraphType()
        {
            var graphType = new PublishedElementInterfaceGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "_id")
                .Which.Type.Should().Be<NonNullGraphType<IdGraphType>>();
        }

//        [Fact]
//        public void ContentTypeFieldResolver_WhenCalled_ReturnsAlias()
//        {
//            var graphType = new PublishedElementInterfaceGraphType();
//
//            var contentType = Substitute.For<IPublishedContentType>();
//            var element = Substitute.For<IPublishedElement>();
//            element.ContentType.Returns(contentType);
//
//            graphType.Fields.Should().Contain(x => x.Name == "_contentType")
//                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = element })
//                .Should().Be(contentType);
//        }
//
//        [Fact]
//        public void CompositionAliasesFieldResolver_WhenCalled_ReturnsAlias()
//        {
//            var graphType = new PublishedElementInterfaceGraphType();
//
//            var element = Substitute.For<IPublishedElement>();
//            element.Key.Returns(new Guid("F14EA3D9-E40A-4A14-B860-492125D6877B"));
//
//            graphType.Fields.Should().Contain(x => x.Name == "_id")
//                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = element })
//                .Should().Be(element.Key);
//        }
    }
}
