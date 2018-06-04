using System;

namespace Our.Umbraco.GraphQL.Filters
{
    public class EqFilter : IFilter
    {
        private readonly object _value;
        private readonly Func<object, object> _valueResolver;

        public EqFilter(Func<object, object> valueResolver, object value)
        {
            _valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            _value = value;
        }

        public bool IsSatisfiedBy(object input)
        {
            var value = _valueResolver(input);
            return Equals(value, _value);
        }
    }
}
