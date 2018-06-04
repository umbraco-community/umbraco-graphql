using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.GraphQL.Filters
{
    public class OrFilter : IFilter
    {
        private readonly IEnumerable<IFilter> _subFilters;

        public OrFilter(IEnumerable<IFilter> subFilters)
        {
            _subFilters = subFilters ?? throw new ArgumentNullException(nameof(subFilters));
        }

        public bool IsSatisfiedBy(object input)
        {
            foreach (var filter in _subFilters)
            {
                if (filter.IsSatisfiedBy(input)) return true;
            }

            return false;
        }
    }
}
