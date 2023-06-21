using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridRowGraphType : ObjectGraphType<JToken>
    {
        public GridRowGraphType()
        {
            Name = "GridRow";
            Field<NonNullGraphType<StringGraphType>>().Name("name")
                .Resolve(context => context.Source.Value<string>("name"));

            Field<NonNullGraphType<ListGraphType<GridAreaGraphType>>>().Name("areas")
                .Resolve(context => context.Source["areas"]);

            Field<NonNullGraphType<GridStylesGraphType>>().Name("styles")
                .Resolve(context => context.Source.Value<GridStylesGraphType>("styles") ?? new GridStylesGraphType());

            Field<NonNullGraphType<GridConfigGraphType>>().Name("config")
                .Resolve(context => context.Source.Value<GridConfigGraphType>("config") ?? new GridConfigGraphType());

        }
    }
}