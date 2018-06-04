using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridEditorGraphType : ObjectGraphType<JToken>
    {
        public GridEditorGraphType()
        {
            Name = "GridEditor";

            Field<NonNullGraphType<StringGraphType>>(
                "name",
                resolve: context => context.Source["name"]
            );
            Field<NonNullGraphType<StringGraphType>>(
                "alias",
                resolve: context => context.Source["alias"]
            );
            Field<NonNullGraphType<StringGraphType>>(
                "view",
                resolve: context => context.Source["view"]
            );
            Field<NonNullGraphType<GridConfigGraphType>>(
                "config",
                resolve: context => context.Source.GetValueOrDefault("config", new { })
            );
        }
    }
}