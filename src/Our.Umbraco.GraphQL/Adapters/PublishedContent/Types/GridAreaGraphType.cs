using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridAreaGraphType : ObjectGraphType<JToken>
    {
        public GridAreaGraphType()
        {
            Name = "GridArea";
            Field<NonNullGraphType<IntGraphType>>().Name("grid")
                .Resolve(context => context.Source.Value<int>("grid"));
            Field<NonNullGraphType<GridStylesGraphType>>().Name("styles")
                .Resolve(context => context.Source.Value<GridStylesGraphType>("styles") ?? new GridStylesGraphType());
            Field<NonNullGraphType<ListGraphType<GridControlGraphType>>>().Name("controls")
                .Resolve(context => context.Source["controls"]);
        }
    }
}