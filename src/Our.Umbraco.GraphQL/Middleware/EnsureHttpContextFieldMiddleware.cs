using System.Threading.Tasks;
using System.Web;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Middleware
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
