using System;

namespace Our.Umbraco.GraphQL.Filters
{
    public class EndsWithFilter : IFilter
    {
        private readonly string _value;
        private readonly Func<object, object> _valueResolver;

        public EndsWithFilter(Func<object, object> valueResolver, object value)
        {
            _valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            _value = Convert.ToString(value);
        }

        public bool IsSatisfiedBy(object input)
        {
            var value = _valueResolver(input);
            if (value == null || _value == null)
            {
                return false;
            }
            return Convert.ToString(value).EndsWith(_value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
