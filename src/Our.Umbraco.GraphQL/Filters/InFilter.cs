using System;

namespace Our.Umbraco.GraphQL.Filters
{
    public class InFilter : IFilter
    {
        private readonly object[] _value;
        private readonly Func<object, object> _valueResolver;

        public InFilter(Func<object, object> valueResolver, object value)
        {
            _valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            _value = (object[]) value;
        }

        public bool IsSatisfiedBy(object input)
        {
            var value = _valueResolver(input);
            return Array.Exists(_value, x => Equals(x, value));
        }
    }
}
