﻿using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridStylesGraphType : StringGraphType
    {
        public GridStylesGraphType()
        {
            Name = "GridStyles";
            // TODO: Maybe extend this?
        }
    }
}