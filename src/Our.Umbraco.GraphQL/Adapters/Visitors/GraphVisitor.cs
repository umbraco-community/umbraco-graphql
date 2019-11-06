using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Visitors
{
    public abstract class GraphVisitor : IGraphVisitor
    {
        public virtual void Visit(EnumerationGraphType graphType)
        {
        }

        public virtual void Visit(IInputObjectGraphType graphType)
        {
        }

        public virtual void Visit(IInterfaceGraphType graphType)
        {
        }

        public virtual void Visit(IObjectGraphType graphType)
        {
        }

        public virtual void Visit(ISchema schema)
        {
        }
    }
}
