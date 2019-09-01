using System.Reflection;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters
{
    public interface IGraphTypeAdapter
    {
        IGraphType Adapt<T>();
        IGraphType Adapt(TypeInfo typeInfo);
    }
}
