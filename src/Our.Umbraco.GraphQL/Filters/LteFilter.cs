using System;
using System.Collections;

namespace Our.Umbraco.GraphQL.Filters
{
    public class LteFilter : IFilter
    {
        private readonly object _value;
        private readonly Func<object, object> _valueResolver;

        public LteFilter(Func<object, object> valueResolver, object value)
        {
            _valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            _value = value;
        }

        public bool IsSatisfiedBy(object input)
        {
            var value = _valueResolver(input);
            return Comparer.DefaultInvariant.Compare(value, _value) <= 0;
        }
    }
}
