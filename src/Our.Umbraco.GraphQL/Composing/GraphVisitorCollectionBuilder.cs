using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Umbraco.Cms.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class GraphVisitorCollectionBuilder : OrderedCollectionBuilderBase<GraphVisitorCollectionBuilder, GraphVisitorCollection, IGraphVisitor>
    {
        public GraphVisitorCollectionBuilder()
        {
        }

        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Scoped;

        protected override GraphVisitorCollectionBuilder This => this;
    }
}
