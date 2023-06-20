﻿using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridSectionGraphType : ObjectGraphType<JToken>
    {
        public GridSectionGraphType()
        {
            Name = "GridSection";
            Field<NonNullGraphType<IntGraphType>>(
                "grid",
                resolve: context => context.Source.Value<int>("grid")
            );
            Field<NonNullGraphType<ListGraphType<GridRowGraphType>>>(
                "rows",
                resolve: context => context.Source["rows"]
            );
        }
    }
}