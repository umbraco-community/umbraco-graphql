using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridEditorGraphType : ObjectGraphType<JToken>
    {
        public GridEditorGraphType()
        {
            Name = "GridEditor";

            Field<NonNullGraphType<StringGraphType>>(
                "name",
                resolve: context => context.Source.Value<string>("name")
            );
            Field<NonNullGraphType<StringGraphType>>(
                "alias",
                resolve: context => context.Source.Value<string>("alias")
            );
            Field<NonNullGraphType<StringGraphType>>(
                "view",
                resolve: context => context.Source.Value<string>("view")
            );
            Field<NonNullGraphType<GridConfigGraphType>>(
                "config",
                resolve: context => context.Source.Value<GridConfigGraphType>("config") ?? new GridConfigGraphType()
            );
        }
    }
}