using System.Threading.Tasks;
using System.Web;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Middleware;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.FieldMiddleware
{
    internal class EnsureHttpContextFieldMiddleware : IFieldMiddleware
    {
        private readonly HttpContext _httpContext;

        public EnsureHttpContextFieldMiddleware(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor is null) throw new System.ArgumentNullException(nameof(httpContextAccessor));

            _httpContext = httpContextAccessor.HttpContext;
        }

        public Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            if (HttpContext.Current == null)
                HttpContext.Current = _httpContext;

            return next(context);
        }
    }
}
