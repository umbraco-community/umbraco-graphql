using System.Collections.Generic;
using FluentAssertions;
using GraphQL.Types;
using NSubstitute;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.PublishedContent.Types
{
    public class PublishedContentTypeGraphTypeTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Name.Should().Be("PublishedContentType");
        }

        [Fact]
        public void Ctor_AddsFields()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "alias")
                .And.Contain(x => x.Name == "compositionAliases")
                .And.Contain(x => x.Name == "itemType")
                .And.Contain(x => x.Name == "variations");
        }

        [Fact]
        public void AliasFieldResolver_WhenCalled_ReturnsAlias()
        {
            var graphType = new PublishedContentTypeGraphType();

            var contentType = Substitute.For<IPublishedContentType>();
            var contentTypeAlias = "person";
            contentType.Alias.Returns(contentTypeAlias);

            graphType.Fields.Should().Contain(x => x.Name == "alias")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = contentType })
                .Should().Be(contentTypeAlias);
        }

        [Fact]
        public void CompositionAliasesFieldResolver_WhenCalled_ReturnsAlias()
        {
            var graphType = new PublishedContentTypeGraphType();

            var contentType = Substitute.For<IPublishedContentType>();
            var compositionAliases = new HashSet<string> { "contentBase", "navigationBase" };
            contentType.CompositionAliases.Returns(compositionAliases);

            graphType.Fields.Should().Contain(x => x.Name == "compositionAliases")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = contentType })
                .Should().Be(compositionAliases);
        }

        [Fact]
        public void ItemTypeFieldResolver_WhenCalled_ReturnsAlias()
        {
            var graphType = new PublishedContentTypeGraphType();

            var contentType = Substitute.For<IPublishedContentType>();
            contentType.ItemType.Returns(PublishedItemType.Content);

            graphType.Fields.Should().Contain(x => x.Name == "itemType")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = contentType })
                .Should().Be(PublishedItemType.Content);
        }

        [Fact]
        public void VariationsFieldResolver_WhenCalled_ReturnsAlias()
        {
            var graphType = new PublishedContentTypeGraphType();

            var contentType = Substitute.For<IPublishedContentType>();
            contentType.Variations.Returns(ContentVariation.Culture);

            graphType.Fields.Should().Contain(x => x.Name == "variations")
                .Which.Resolver.Resolve(new ResolveFieldContext{ Source = contentType })
                .Should().Be(ContentVariation.Culture);
        }

        [Fact]
        public void AliasField_Type_ShouldBeStringGraphType()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "alias")
                .Which.Type.Should().Be<NonNullGraphType<StringGraphType>>();
        }

        [Fact]
        public void CompositionField_Type_ShouldBeListOfStringGraphType()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "compositionAliases")
                .Which.Type.Should().Be<NonNullGraphType<ListGraphType<StringGraphType>>>();
        }

        [Fact]
        public void ItemTypeField_Type_ShouldBePublishedItemTypeGraphType()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "itemType")
                .Which.Type.Should().Be<NonNullGraphType<PublishedItemTypeGraphType>>();
        }

        [Fact]
        public void VariationsField_Type_ShouldBeContentVariationGraphType()
        {
            var graphType = new PublishedContentTypeGraphType();

            graphType.Fields.Should().Contain(x => x.Name == "variations")
                .Which.Type.Should().Be<NonNullGraphType<ContentVariationGraphType>>();
        }
    }
}
