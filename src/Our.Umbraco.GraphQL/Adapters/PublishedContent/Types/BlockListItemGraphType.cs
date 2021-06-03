using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Types;
using Umbraco.Cms.Core.Models.Blocks;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class BlockListItemGraphType : ObjectGraphType<BlockListItem>
    {
        public BlockListItemGraphType()
        {
            Name = "BlockListItem";

            Field<PublishedElementInterfaceGraphType>("content", resolve: ctx => ctx.Source?.Content);
            Field<UdiGraphType>("contentUdi", resolve: ctx => ctx.Source?.ContentUdi);
            Field<PublishedElementInterfaceGraphType>("settings", resolve: ctx => ctx.Source?.Settings);
            Field<UdiGraphType>("settingsUdi", resolve: ctx => ctx.Source?.SettingsUdi);
        }
    }
}
