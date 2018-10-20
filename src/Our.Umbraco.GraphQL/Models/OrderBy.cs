namespace Our.Umbraco.GraphQL.Models
{
    public class OrderBy
    {
        public string Field { get; }
        public SortOrder Order { get; }

        public OrderBy(string field, SortOrder order)
        {
            Field = field;
            Order = order;
        }
    }
}