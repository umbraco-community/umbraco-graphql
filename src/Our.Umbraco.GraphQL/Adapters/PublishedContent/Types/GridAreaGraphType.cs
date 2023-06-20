using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridAreaGraphType : ObjectGraphType<JToken>
    {
        public GridAreaGraphType()
        {
            Name = "GridArea";
            Field<NonNullGraphType<IntGraphType>>(
                "grid",
                resolve: context => context.Source.Value<int>("grid")
            );
            Field<NonNullGraphType<GridStylesGraphType>>(
                "styles",
                resolve: context => context.Source.Value<GridStylesGraphType>("styles") ?? new GridStylesGraphType()
            );
            Field<NonNullGraphType<ListGraphType<GridControlGraphType>>>(
                "controls",
                resolve: context => context.Source["controls"]
            );
        }
    }
}