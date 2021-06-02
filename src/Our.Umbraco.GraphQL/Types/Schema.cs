namespace Our.Umbraco.GraphQL.Types
{
    public class Schema<TQuery>
    {
        public TQuery Query { get; set; }
    }
    public class Schema<TQuery, TMutation>
    {
        public TQuery Query { get; set; }
        public TMutation Mutation { get; set; }
    }
}
