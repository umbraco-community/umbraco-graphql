using Our.Umbraco.GraphQL.Adapters.Visitors;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class GraphVisitorCollectionBuilder : OrderedCollectionBuilderBase<GraphVisitorCollectionBuilder, GraphVisitorCollection, IGraphVisitor>
    {
        public GraphVisitorCollectionBuilder()
        {
        }

        protected override Lifetime CollectionLifetime => Lifetime.Scope;

        protected override GraphVisitorCollectionBuilder This => this;
    }
}
