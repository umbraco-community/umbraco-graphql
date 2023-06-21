using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridSectionGraphType : ObjectGraphType<JToken>
    {
        public GridSectionGraphType()
        {
            Name = "GridSection";
            Field<NonNullGraphType<IntGraphType>>().Name("grid")
                .Resolve(context => context.Source.Value<int>("grid"));

            Field<NonNullGraphType<ListGraphType<GridRowGraphType>>>().Name("rows")
                .Resolve(context => context.Source["rows"]);

        }
    }
}