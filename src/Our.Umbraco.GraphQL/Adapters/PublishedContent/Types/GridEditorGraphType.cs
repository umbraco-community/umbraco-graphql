using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridEditorGraphType : ObjectGraphType<JToken>
    {
        public GridEditorGraphType()
        {
            Name = "GridEditor";

            Field<NonNullGraphType<StringGraphType>>().Name("name")
                .Resolve(context => context.Source.Value<string>("name"));

            Field<NonNullGraphType<StringGraphType>>().Name("alias")
                .Resolve(context => context.Source.Value<string>("alias"));

            Field<NonNullGraphType<StringGraphType>>().Name("view")
                .Resolve(context => context.Source.Value<string>("view"));

            Field<NonNullGraphType<GridConfigGraphType>>().Name("config")
                .Resolve(context => context.Source.Value<GridConfigGraphType>("config") ?? new GridConfigGraphType());

        }
    }
}