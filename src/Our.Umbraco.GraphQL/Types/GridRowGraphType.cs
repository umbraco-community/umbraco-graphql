using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridRowGraphType : ObjectGraphType<JToken>
    {
        public GridRowGraphType()
        {
            Name = "GridRow";
            Field<NonNullGraphType<StringGraphType>>(
                "name",
                resolve: context => context.Source["name"]
            );
            Field<NonNullGraphType<ListGraphType<GridAreaGraphType>>>(
                "areas",
                resolve: context => context.Source["areas"]
            );
            Field<NonNullGraphType<GridStylesGraphType>>(
                "styles",
                resolve: context => context.Source.GetValueOrDefault("styles", new {})
            );
            Field<NonNullGraphType<GridConfigGraphType>>(
                "config",
                resolve: context => context.Source.GetValueOrDefault("config", new { })
            );
        }
    }
}