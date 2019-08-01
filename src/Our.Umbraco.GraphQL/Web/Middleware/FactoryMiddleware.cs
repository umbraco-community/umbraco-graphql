using LightInject;
using Microsoft.Owin;
using System.Threading.Tasks;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.Web.Middleware
{
    internal class FactoryMiddleware : OwinMiddleware
    {
        private readonly IFactory _factory;

        public FactoryMiddleware(OwinMiddleware next, IFactory factory)
            : base(next)
        {
            _factory = factory ?? throw new System.ArgumentNullException(nameof(factory));
        }

        public async override Task Invoke(IOwinContext context)
        {
            using (var scope = _factory.BeginScope())
            {
                context.Set("umbraco:factory", _factory);
                await Next.Invoke(context);
            }
        }
    }
}
