using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Visitors
{
    public interface IGraphVisitor
    {
        void Visit(EnumerationGraphType graphType);
        void Visit(IInputObjectGraphType graphType);
        void Visit(IInterfaceGraphType graphType);
        void Visit(IObjectGraphType graphType);
        void Visit(ISchema schema);
    }
}
