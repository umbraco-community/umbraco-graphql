using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridAreaGraphType : ObjectGraphType<JToken>
    {
        public GridAreaGraphType()
        {
            Name = "GridArea";
            Field<NonNullGraphType<IntGraphType>>(
                "grid",
                resolve: context => context.Source["grid"]
            );
            Field<NonNullGraphType<GridStylesGraphType>>(
                "styles",
                resolve: context => context.Source.GetValueOrDefault("styles", new { })
            );
            Field<NonNullGraphType<ListGraphType<GridControlGraphType>>>(
                "controls",
                resolve: context => context.Source["controls"]
            );
        }
    }
}