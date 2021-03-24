using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.GraphQL.Composing
{
    public static class CompositionExtensions
    {
        public static GraphVisitorCollectionBuilder GraphVisitors(this IUmbracoBuilder builder) =>
            builder.WithCollectionBuilder<GraphVisitorCollectionBuilder>();

        public static FieldMiddlewareCollectionBuilder FieldMiddlewares(this IUmbracoBuilder builder) =>
            builder.WithCollectionBuilder<FieldMiddlewareCollectionBuilder>();
    }
}
