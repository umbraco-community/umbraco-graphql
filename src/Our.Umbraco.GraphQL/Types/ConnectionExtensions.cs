using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{
    public static class ConnectionExtensions
    {
        public static Connection<TSource> ToConnection<TSource, TParent>(this IEnumerable<TSource> source,
            ResolveConnectionContext<TParent> context)
        {
            var list = source.ToList();
            return ToConnection(list, context, list.Count);
        }

        public static Connection<TSource> ToConnection<TSource, TParent>(this IEnumerable<TSource> source,
            ResolveConnectionContext<TParent> context, int totalCount)
        {
            var items = source as List<TSource> ?? source.ToList();

            var after = context.After;
            var before = context.Before;
            var first = context.First;
            var last = context.Last;

            int startOffset = 0;
            int endOffset = items.Count;

            if (after != null)
            {
                // cursor starts at 0, so we need to add one to the offset to ensure we don't include the same item
                startOffset = CursorToOffset(after) + 1;
            }
            if(before != null)
            {
                endOffset = CursorToOffset(before);
            }
            if (first.HasValue)
            {
                endOffset = startOffset + first.Value;
            }
            if (last.HasValue)
            {
                startOffset = endOffset - last.Value;
            }

            // Ensure startOffset is not negative
            startOffset = Math.Max(0, startOffset);

            var edges = source.Skip(startOffset).Take(endOffset - startOffset).Select((x, i) => new Edge<TSource>
            {
                Cursor = OffsetToCursor(startOffset + i),
                Node = x
            }).ToList();

            return new Connection<TSource>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    EndCursor = edges.LastOrDefault()?.Cursor,
                    HasNextPage = endOffset < items.Count,
                    HasPreviousPage = startOffset > 0,
                    StartCursor = edges.FirstOrDefault()?.Cursor
                }
            };
        }

        public static int CursorToOffset(string cursor)
        {
            return int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Substring("connection:".Length));
        }

        public static string OffsetToCursor(int offset)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"connection:{offset}"));
        }
    }
}
