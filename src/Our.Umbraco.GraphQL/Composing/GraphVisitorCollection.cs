using System;
using System.Collections.Generic;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Umbraco.Cms.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class GraphVisitorCollection : BuilderCollectionBase<IGraphVisitor>
    {
        public GraphVisitorCollection(Func<IEnumerable<IGraphVisitor>> items)
            : base(items)
        {
        }
    }
}
