using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridGraphType : ObjectGraphType<JToken>
    {
        public GridGraphType()
        {
            Name = "Grid";
            Field<NonNullGraphType<StringGraphType>>(
                "name",
                resolve: context => context.Source["name"]
            );
            Field<NonNullGraphType<ListGraphType<GridSectionGraphType>>>(
                "sections",
                resolve: context => context.Source["sections"]
            );
        }
    }
}