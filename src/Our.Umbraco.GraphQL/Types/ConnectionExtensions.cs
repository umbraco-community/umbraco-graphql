using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Our.Umbraco.GraphQL.Types.Relay;

namespace Our.Umbraco.GraphQL.Types
{
    public static class ConnectionExtensions
    {
        public static Connection<TSource> ToConnection<TSource>(this IEnumerable<TSource> source, Func<TSource, object> idSelector,
             int? first = null, string after = null, int? last = null, string before = null, long? totalCount = null)
        {
            if(first < 0)
                throw new ArgumentException($"{nameof(first)} cannot be less than 0.", nameof(first));

            if(last < 0)
                throw new ArgumentException($"{nameof(last)} cannot be less than 0.", nameof(last));

            var sourceList = source.ToList();

            var edges = sourceList.Select(x => new Edge<TSource>
            {
                Cursor = IdToCursor(idSelector(x)),
                Node = x
            });

            if (after != null) edges = edges.SkipWhile(x => x.Cursor != after).Skip(1);
            if (before != null) edges = edges.TakeWhile(x => x.Cursor != before);
            if (first.HasValue) edges = edges.Take(first.Value);
            if (last.HasValue) edges = edges.Reverse().Take(last.Value).Reverse();

            var edgeList = edges.ToList();
            var endCursor = edgeList.LastOrDefault()?.Cursor;
            var startCursor = edgeList.FirstOrDefault()?.Cursor;

            var firstCursor = sourceList.Count > 0 ? IdToCursor(idSelector(sourceList.First())) : null;
            var lastCursor = sourceList.Count > 0 ? IdToCursor(idSelector(sourceList.Last())) : null;

            return new Connection<TSource>
            {
                Edges = edgeList,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    EndCursor = endCursor,
                    HasNextPage = endCursor != lastCursor,
                    HasPreviousPage = startCursor != firstCursor,
                    StartCursor = startCursor
                }
            };
        }

        private static string IdToCursor(object id) => IdToCursor(new Id(id.ToString()));

        private static string IdToCursor(Id id) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"connection:{id}"));
    }
}
