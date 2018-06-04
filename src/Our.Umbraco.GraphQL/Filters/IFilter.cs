namespace Our.Umbraco.GraphQL.Filters
{
    public interface IFilter
    {
        bool IsSatisfiedBy(object input);
    }
}
