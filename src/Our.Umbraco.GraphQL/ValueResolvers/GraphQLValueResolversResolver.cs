using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.ObjectResolution;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class GraphQLValueResolversResolver : ManyObjectsResolverBase<GraphQLValueResolversResolver, IGraphQLValueResolver>
    {
        internal GraphQLValueResolversResolver(IServiceProvider serviceProvider, ILogger logger,
            IEnumerable<Type> value, ObjectLifetimeScope scope = ObjectLifetimeScope.Application) : base(
            serviceProvider, logger, value, scope)
        {
        }

        public IEnumerable<IGraphQLValueResolver> Converters => Values;

        internal IGraphQLValueResolver FindResolver(PublishedPropertyType propertyType)
        {
            // TODO: Validate multiple found and throw better exceptions
            return Values.SingleOrDefault(x => x.IsConverter(propertyType));
        }
    }
}
