using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Composing;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class GraphQLValueResolverCollection : BuilderCollectionBase<IGraphQLValueResolver>
    {
        public GraphQLValueResolverCollection(IEnumerable<IGraphQLValueResolver> items)
            : base(items)
        { }

        private readonly object _locker = new object();

        private List<IGraphQLValueResolver> _defaultResolvers;
        private IGraphQLValueResolver _fallbackResolver;

        private List<IGraphQLValueResolver> DefaultResolvers
        {
            get
            {
                lock (_locker)
                {
                    if (_defaultResolvers != null)
                    {
                        return _defaultResolvers;
                    }

                    _defaultResolvers = new List<IGraphQLValueResolver>();

                    foreach (var resolver in this)
                    {
                        var attr = resolver.GetType().GetCustomAttribute<DefaultGraphQLValueResolverAttribute>(false);
                        if (attr != null)
                        {
                            _defaultResolvers.Add(resolver);
                        }
                    }

                    return _defaultResolvers;
                }
            }
        }

        internal IGraphQLValueResolver FallbackResolver
        {
            get
            {
                lock(_locker)
                {
                    if(_fallbackResolver != null)
                    {
                        return _fallbackResolver;
                    }

                    _fallbackResolver = this.Single(x => x is DefaultValueResolver);
                }

                return _fallbackResolver;
            }
        }

        internal bool IsDefault(IGraphQLValueResolver converter)
            => DefaultResolvers.Contains(converter);
    }
}
