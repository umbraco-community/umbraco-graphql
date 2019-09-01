using System.Reflection;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Types.Resolution
{
    public interface ITypeRegistry
    {
        void Add<TType, TGraphType>() where TGraphType : IGraphType;
        TypeInfo Get<TType>();
        TypeInfo Get(TypeInfo type);
    }
}
