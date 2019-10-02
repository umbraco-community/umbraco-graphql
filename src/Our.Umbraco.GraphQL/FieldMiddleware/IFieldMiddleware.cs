using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Middleware
{
    public interface IFieldMiddleware
    {
        Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next);
    }
}
