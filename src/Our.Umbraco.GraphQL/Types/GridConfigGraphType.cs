using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridConfigGraphType : StringGraphType
    {
        public GridConfigGraphType()
        {
            Name = "GridConfig";
            // TODO: Maybe extend this?
        }
    }
}