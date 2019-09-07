using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public static class CompositionExtensions
    {
        public static GraphVisitorCollectionBuilder GraphVisitors(this Composition composition) =>
            composition.WithCollectionBuilder<GraphVisitorCollectionBuilder>();
    }
}
