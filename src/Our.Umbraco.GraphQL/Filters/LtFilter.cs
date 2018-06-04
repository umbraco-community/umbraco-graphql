using System;
using System.Collections;

namespace Our.Umbraco.GraphQL.Filters
{
    public class LtFilter : IFilter
    {
        private readonly object _value;
        private readonly Func<object, object> _valueResolver;

        public LtFilter(Func<object, object> valueResolver, object value)
        {
            _valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            _value = value;
        }

        public bool IsSatisfiedBy(object input)
        {
            var value = _valueResolver(input);
            return Comparer.DefaultInvariant.Compare(value, _value) < 0;
        }
    }
}
