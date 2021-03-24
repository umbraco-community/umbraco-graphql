using System;
using FluentAssertions;
using GraphQL.Types;
using NSubstitute;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Xunit;
using IdGraphType = Our.Umbraco.GraphQL.Adapters.Types.IdGraphType;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class PublishedElementGraphTypeTests
    {
        private PublishedElementGraphType CreateSUT(IContentTypeComposition contentType = null)
        {
            return new PublishedElementGraphType(contentType ?? Substitute.For<IContentTypeComposition>(),
                Substitute.For<IPublishedContentType>(), new TypeRegistry());
        }

        [Fact]
        public void Ctor_SetsName()
        {
            var contentType = Substitute.For<IContentType>();
            contentType.Alias.Returns("feature");

            var graphType = CreateSUT(contentType);

            graphType.Name.Should().Be("FeaturePublishedElement");
        }

        [Fact]
        public void IsOfType_WhenTypeMatches_ReturnsTrue()
        {
            var contentType = Substitute.For<IContentType>();
            contentType.Alias.Returns("person");
            var content = Substitute.For<IPublishedElement>();
            content.ContentType.Alias.Returns("person");

            var graphType = CreateSUT(contentType);

            graphType.IsTypeOf(content).Should().BeTrue();
        }

        [Fact]
        public void IsOfType_WhenTypeDoesNotMatch_ReturnsFalse()
        {
            var content = Substitute.For<IPublishedElement>();
            content.ContentType.Alias.Returns("person");

            var graphType = CreateSUT();

            graphType.IsTypeOf(content).Should().BeFalse();
        }

        [Fact]
        public void Interfaces_ContainsPublishedElementInterfaceGraphType()
        {
            var graphType = CreateSUT();

            graphType.Interfaces.Should().Contain(typeof(PublishedElementInterfaceGraphType));
        }

        [Theory]
        [InlineData("_contentType", typeof(NonNullGraphType<PublishedContentTypeGraphType>))]
        [InlineData("_id", typeof(NonNullGraphType<IdGraphType>))]
        public void Fields_Type_ShouldBeOfExpectedType(string field, Type type)
        {
            var graphType = CreateSUT();

            graphType.Fields.Should().Contain(x => x.Name == field)
                .Which.Type.Should().Be(type);
        }

        [Fact]
        public void ContentTypeFieldResolver_WhenCalled_ReturnsAlias()
        {
            var graphType = CreateSUT();

            var contentType = Substitute.For<IPublishedContentType>();
            var element = Substitute.For<IPublishedElement>();
            element.ContentType.Returns(contentType);

            graphType.Fields.Should().Contain(x => x.Name == "_contentType")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = element })
                .Should().Be(contentType);
        }

        [Fact]
        public void IdFieldResolver_WhenCalled_ReturnsId()
        {
            var graphType = CreateSUT();

            var element = Substitute.For<IPublishedElement>();
            element.Key.Returns(new Guid("F14EA3D9-E40A-4A14-B860-492125D6877B"));

            graphType.Fields.Should().Contain(x => x.Name == "_id")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = element })
                .Should().Be(new Id(element.Key.ToString()));
        }
    }
}
