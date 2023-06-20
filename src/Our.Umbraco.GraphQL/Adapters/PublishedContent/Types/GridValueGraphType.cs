using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridValueGraphType : StringGraphType
    {
        public GridValueGraphType()
        {
            Name = "GridValue";
        }
    }
}