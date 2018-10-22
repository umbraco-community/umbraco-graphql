using LightInject;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class GraphQLValueResolverCollectionBuilder : OrderedCollectionBuilderBase<GraphQLValueResolverCollectionBuilder, GraphQLValueResolverCollection, IGraphQLValueResolver>
    {
        public GraphQLValueResolverCollectionBuilder(IServiceContainer container)
            : base(container)
        { }

        protected override GraphQLValueResolverCollectionBuilder This => this;
    }
}
