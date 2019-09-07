using System;
using FluentAssertions;
using NSubstitute;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedCache;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types.PublishedContent
{
    public class PublishedContentQueryTests
    {
        private PublishedContentQuery CreateSUT() => new PublishedContentQuery();

        [Fact]
        public void AtRoot_WhenCalled_ReturnsPublishedContentAtRootQuery()
        {
            var query = CreateSUT();
            var expected = new PublishedContentAtRootQuery(Substitute.For<IPublishedSnapshotAccessor>());

            var actual = query.AtRoot(expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ById_WhenContentExists_ReturnsPublishedContent()
        {
            var query = CreateSUT();
            var content = Substitute.For<IPublishedContent>();
            content.Key.Returns(new Guid("3B1E9E9C-A89E-4FE0-902D-3466D8678E47"));

            var snapshotAccessor = Substitute.For<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot.Content.GetById(Arg.Is(content.Key))
                .Returns(content);

            var actual = query.ById(snapshotAccessor, new Id(content.Key.ToString()));

            actual.Should().Be(content);
        }

        [Fact]
        public void ById_WhenContentDoesNotExists_ReturnsNull()
        {
            var query = CreateSUT();
            var id = new Guid("3B1E9E9C-A89E-4FE0-902D-3466D8678E47");

            var snapshotAccessor = Substitute.For<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot.Content.GetById(Arg.Is(id))
                .Returns((IPublishedContent)null);

            var actual = query.ById(snapshotAccessor, new Id(id.ToString()));

            actual.Should().BeNull();
        }

        [Fact]
        public void ByType_WhenCalled_ReturnsPublishedContentByTypeQuery()
        {
            var query = CreateSUT();
            var expected = new PublishedContentByTypeQuery();

            var actual = query.ByType(expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ByUrl_WhenContentExists_ReturnsPublishedContent()
        {
            var query = CreateSUT();
            var content = Substitute.For<IPublishedContent>();
            content.Key.Returns(new Guid("3B1E9E9C-A89E-4FE0-902D-3466D8678E47"));

            var snapshotAccessor = Substitute.For<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot.Content.GetByRoute("/")
                .Returns(content);

            var actual = query.ByUrl(snapshotAccessor, "/");

            actual.Should().Be(content);
        }

        [Fact]
        public void ByUrl_WhenContentDoesNotExists_ReturnsNull()
        {
            var query = CreateSUT();

            var snapshotAccessor = Substitute.For<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot.Content.GetByRoute("/")
                .Returns((IPublishedContent)null);

            var actual = query.ByUrl(snapshotAccessor, "/");

            actual.Should().BeNull();
        }

        [Fact]
        public void ExtendUmbracoQueryWithPublishedContentQuery_Content_WhenCalled_ReturnsPublishedContentQuery()
        {
            var query = new ExtendUmbracoQueryWithPublishedContentQuery();
            var expected = CreateSUT();

            var actual = query.Content(expected);

            actual.Should().Be(expected);
        }
    }
}
