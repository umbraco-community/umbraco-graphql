using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.GraphQL.Types.Relay
{
    public class Connection<T>
    {
        public long? TotalCount { get; set; }
        public PageInfo PageInfo { get; set; }
        public List<Edge<T>> Edges { get; set; }
        public List<T> Items => Edges?.Select(x => x.Node).ToList();
    }
}
