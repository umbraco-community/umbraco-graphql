namespace Our.Umbraco.GraphQL.Types
{
    public class Schema<TQuery>
    {
        public TQuery Query { get; set; }
    }
}
