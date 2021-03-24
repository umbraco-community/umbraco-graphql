using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.GraphQL.Middleware;
using Umbraco.Cms.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class FieldMiddlewareCollectionBuilder : OrderedCollectionBuilderBase<FieldMiddlewareCollectionBuilder, FieldMiddlewareCollection, IFieldMiddleware>
    {
        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Scoped;

        protected override FieldMiddlewareCollectionBuilder This => this;
    }
}
