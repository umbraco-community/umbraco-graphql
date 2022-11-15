using System;
using Our.Umbraco.GraphQL.Attributes;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace Our.Umbraco.GraphQL.Types.PublishedContent
{
    public class PublishedContentQuery
    {
        [NonNull]
        public PublishedContentAtRootQuery AtRoot([Inject] PublishedContentAtRootQuery query) => query;

        public IPublishedContent ById([Inject] IPublishedSnapshotAccessor snapshotAccessor, Id id, string culture = null) =>
            GetInternal(snapshotAccessor, x => x.GetById(id.As<Guid>()), culture);

        [NonNull]
        public PublishedContentByTypeQuery ByType([Inject] PublishedContentByTypeQuery query) => query;

        public IPublishedContent ByUrl([Inject] IPublishedSnapshotAccessor snapshotAccessor, string url, string culture = null) =>
            GetInternal(snapshotAccessor, x => x.GetByRoute(url, culture: culture), culture);

        private static IPublishedContent GetInternal(IPublishedSnapshotAccessor snapshotAccessor, Func<IPublishedContentCache, IPublishedContent> fetch, string culture)
        {
            if (!snapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
            {
                throw new InvalidOperationException("Wasn't possible to a get a valid Snapshot");
            }

            var content = fetch(publishedSnapshot.Content);
            if (culture == null || content != null && content.IsInvariantOrHasCulture(culture))
                return content;

            return null;
        }
    }

    public class ExtendUmbracoQueryWithPublishedContentQuery
    {
        [NonNull]
        [Description("Published content in Umbraco")]
        public PublishedContentQuery Content([Inject] PublishedContentQuery query) => query;
    }
}
