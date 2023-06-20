using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridRowGraphType : ObjectGraphType<JToken>
    {
        public GridRowGraphType()
        {
            Name = "GridRow";
            Field<NonNullGraphType<StringGraphType>>(
                "name",
                resolve: context => context.Source.Value<string>("name")
            );
            Field<NonNullGraphType<ListGraphType<GridAreaGraphType>>>(
                "areas",
                resolve: context => context.Source["areas"]
            );
            Field<NonNullGraphType<GridStylesGraphType>>(
                "styles",
                resolve: context => context.Source.Value<GridStylesGraphType>("styles") ?? new GridStylesGraphType()
            );
            Field<NonNullGraphType<GridConfigGraphType>>(
                "config",
                resolve: context => context.Source.Value<GridConfigGraphType>("config") ?? new GridConfigGraphType()
            );
        }
    }
}