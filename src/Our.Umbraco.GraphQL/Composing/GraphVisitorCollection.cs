using System.Collections.Generic;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Composing
{
    public class GraphVisitorCollection : BuilderCollectionBase<IGraphVisitor>
    {
        public GraphVisitorCollection(IEnumerable<IGraphVisitor> items)
            : base(items)
        {
        }
    }
}
