using FluentAssertions;
using NSubstitute;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedCache;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Types.PublishedContent
{
    public class PublishedContentAtRootQueryTests
    {
        private PublishedContentAtRootQuery CreateSUT(IPublishedSnapshotAccessor snapshotAccessor)
        {
            return new PublishedContentAtRootQuery(snapshotAccessor);
        }

        [Fact]
        public void All_WhenCalled_ReturnsRootContent()
        {
            var items = new[]
            {
                Substitute.For<IPublishedContent>(),
                Substitute.For<IPublishedContent>(),
                Substitute.For<IPublishedContent>(),
            };

            var snapshotAccessor = Substitute.For<IPublishedSnapshotAccessor>();
            snapshotAccessor.PublishedSnapshot.Content.GetAtRoot()
                .Returns(items);
            var query = CreateSUT(snapshotAccessor);

            var results = query.All();

            results.Items.Should().Equal(items);
        }
    }
}
