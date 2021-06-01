using System.Threading.Tasks;
using GraphQL;
using GraphQL.Instrumentation;

namespace Our.Umbraco.GraphQL.Middleware
{
    public interface IFieldMiddleware
    {
        Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next);
    }
}
