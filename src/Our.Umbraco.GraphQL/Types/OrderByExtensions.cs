using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public static class OrderByExtensions
    {
        public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, IEnumerable<OrderBy> orderBy)
        {
            if (orderBy == null) return source;

            foreach (var order in orderBy)
            {
                if (source is IOrderedEnumerable<TSource> ordered)
                {
                    source = order.Direction == SortOrder.Ascending
                        ? ordered.ThenBy(order.Resolve)
                        : ordered.ThenByDescending(order.Resolve);
                }
                else
                {
                    source = order.Direction == SortOrder.Ascending
                        ? source.OrderBy(order.Resolve)
                        : source.OrderByDescending(order.Resolve);
                }
            }

            return source;
        }
    }
}
