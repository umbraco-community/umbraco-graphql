using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types.Relay;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;

namespace Our.Umbraco.GraphQL.Types.PublishedContent
{
    public class PublishedContentAtRootQuery
    {
        private readonly IPublishedSnapshotAccessor _snapshotAccessor;

        public PublishedContentAtRootQuery(IPublishedSnapshotAccessor snapshotAccessor)
        {
            _snapshotAccessor = snapshotAccessor ?? throw new ArgumentNullException(nameof(snapshotAccessor));
        }

        
        [NonNull, NonNullItem]
        public Connection<IPublishedContent> All(string culture = null, int? first = null, string after = null,
            int? last = null, string before = null, IEnumerable<OrderBy> orderBy = null)
        {
            if (!_snapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
            {
                throw new InvalidOperationException("Wasn't possible to a get a valid Snapshot");
            }

            var rootContent = publishedSnapshot.Content.GetAtRoot(culture).OrderBy(orderBy).ToList();
            return rootContent.ToConnection(x => x.Key, first, after, last, before, rootContent.Count);
        }
    }
}
