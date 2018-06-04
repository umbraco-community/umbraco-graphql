using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridValueGraphType : StringGraphType
    {
        public GridValueGraphType()
        {
            Name = "GridValue";
        }
    }
}