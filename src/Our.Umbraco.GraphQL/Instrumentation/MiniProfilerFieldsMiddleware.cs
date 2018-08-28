using GraphQL.Instrumentation;
using GraphQL.Types;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Instrumentation
{
    public class MiniProfilerFieldsMiddleware
    {
        public Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            using (MiniProfiler.Current.Step($"[GraphQL] Resolving {string.Join(".", context.Path)}", ProfileLevel.Info))
            {
                return next(context);
            }
        }
    }
}
