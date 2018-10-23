using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.ObjectResolution;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class GraphQLValueResolversResolver : ManyObjectsResolverBase<GraphQLValueResolversResolver, IGraphQLValueResolver>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private Tuple<IGraphQLValueResolver, DefaultGraphQLValueResolverAttribute>[] _defaults = null;
        private IGraphQLValueResolver _fallback = null;

        internal GraphQLValueResolversResolver(IServiceProvider serviceProvider, ILogger logger,
            IEnumerable<Type> value, ObjectLifetimeScope scope = ObjectLifetimeScope.Application) : base(
            serviceProvider, logger, value, scope)
        {
        }

        public IEnumerable<IGraphQLValueResolver> Resolvers => Values;

        /// <summary>
        /// Caches and gets the default resolvers with their metadata
        /// </summary>
        internal Tuple<IGraphQLValueResolver, DefaultGraphQLValueResolverAttribute>[] DefaultResolvers
        {
            get
            {
                using (var locker = new UpgradeableReadLock(_lock))
                {
                    if (_defaults == null)
                    {
                        locker.UpgradeToWriteLock();

                        var defaultResolverWithAttributes = Resolvers
                            .Select(x => new
                            {
                                attribute = x.GetType().GetCustomAttribute<DefaultGraphQLValueResolverAttribute>(false),
                                resolver = x
                            })
                            .Where(x => x.attribute != null)
                            .ToArray();

                        _defaults = defaultResolverWithAttributes
                            .Select(
                                x => new Tuple<IGraphQLValueResolver, DefaultGraphQLValueResolverAttribute>(x.resolver, x.attribute))
                            .ToArray();
                    }

                    return _defaults;
                }
            }
        }

        /// <summary>
        /// Caches and gets the fallback resolver
        /// </summary>
        internal IGraphQLValueResolver FallbackResolver
        {
            get
            {
                using (var locker = new UpgradeableReadLock(_lock))
                {
                    if (_fallback == null)
                    {
                        locker.UpgradeToWriteLock();

                        _fallback = Resolvers.Single(x => x is DefaultValueResolver);
                    }

                    return _fallback;
                }
            }
        }
    }
}
