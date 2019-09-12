using Our.Umbraco.GraphQL.Middleware;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class FieldMiddlewareCollectionBuilder : OrderedCollectionBuilderBase<FieldMiddlewareCollectionBuilder, FieldMiddlewareCollection, IFieldMiddleware>
    {
        protected override Lifetime CollectionLifetime => Lifetime.Scope;

        protected override FieldMiddlewareCollectionBuilder This => this;
    }
}
