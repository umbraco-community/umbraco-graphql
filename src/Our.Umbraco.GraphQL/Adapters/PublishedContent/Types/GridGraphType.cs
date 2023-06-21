using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridGraphType : ObjectGraphType<JToken>
    {
        public GridGraphType()
        {
            Name = "Grid";

            Field<NonNullGraphType<StringGraphType>>().Name("name")
                .Resolve(context => context.Source.Value<string>("name"));

            Field<NonNullGraphType<ListGraphType<GridSectionGraphType>>>().Name("sections")
                .Resolve(context => context.Source["sections"]);


        }
    }
}