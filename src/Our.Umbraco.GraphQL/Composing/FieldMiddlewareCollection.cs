using System.Collections.Generic;
using Our.Umbraco.GraphQL.Middleware;
using Umbraco.Cms.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class FieldMiddlewareCollection : BuilderCollectionBase<IFieldMiddleware>
    {
        public FieldMiddlewareCollection(IEnumerable<IFieldMiddleware> items)
            : base(items)
        {
        }
    }
}
