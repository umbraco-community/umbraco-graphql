using System;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class OrderBy
    {
        private readonly Func<ResolveFieldContext, object> _resolver;

        internal OrderBy(string field, SortOrder direction, Func<ResolveFieldContext, object> resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Direction = direction;
        }

        public string Field { get; }
        public SortOrder Direction { get; }

        public object Resolve<TSource>(TSource source)
        {
            return _resolver(new ResolveFieldContext
            {
                Source = source,
            });
        }
    }
}
