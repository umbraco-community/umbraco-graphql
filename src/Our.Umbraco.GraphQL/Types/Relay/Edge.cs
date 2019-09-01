namespace Our.Umbraco.GraphQL.Types.Relay
{
    public class Edge<T>
    {
        public string Cursor { get; set; }
        public T Node { get; set; }
    }
}
