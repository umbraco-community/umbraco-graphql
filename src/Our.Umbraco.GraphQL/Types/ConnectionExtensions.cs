using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;

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
            // TODO: Implement paging logic
            var items = source.Select(x => new Edge<TSource>
            {
                Node = x
            }).ToList();

            var after = context.After;
            var before = context.Before;
            var first = context.First;
            var last = context.Last;

            return new Connection<TSource>
            {
                Edges = items,
                PageInfo = new PageInfo
                {
                }
            };
        }
    }
}
