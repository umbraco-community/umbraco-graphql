using System;

namespace Our.Umbraco.GraphQL.Filters
{
    public class NotFilter : IFilter
    {
        private readonly IFilter _filter;

        public NotFilter(IFilter filter)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public bool IsSatisfiedBy(object input)
        {
            return false == _filter.IsSatisfiedBy(input);
        }
    }
}
