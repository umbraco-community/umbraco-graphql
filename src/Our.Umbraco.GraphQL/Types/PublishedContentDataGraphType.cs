using GraphQL.Types;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentDataGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentDataGraphType()
        {
            Name = "PublishedContentData";

            //TODO: black/whitelist properties
            Field<NonNullGraphType<DateGraphType>>("createDate", "Create date of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("creatorName", "Name of the content creator.").SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("documentTypeAlias", "Document type alias of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<IdGraphType>>("id", "Unique id of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<IntGraphType>>("index", "Index of the content.", resolve: context => context.Source.GetIndex()).SetPermissions(this, true);
            Field<NonNullGraphType<IntGraphType>>("level", "Level of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<BooleanGraphType>>("isFirst", "Is the content first in the list.", resolve: context => context.Source.IsFirst()).SetPermissions(this, true);
            Field<NonNullGraphType<BooleanGraphType>>("isLast", "Is the content last in the list.", resolve: context => context.Source.IsLast()).SetPermissions(this, true);
            Field<NonNullGraphType<BooleanGraphType>>("isVisible", "Is the content visible.", resolve: context => context.Source.IsVisible()).SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("name", "Name of the content.").SetPermissions(this, true);
            Field<PublishedContentInterfaceGraphType>("parent", "Parent of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<IntGraphType>>("sortOrder", "SortOrder of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<DateGraphType>>("updateDate", "Update date of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("url", "Url of the content.").SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("urlAbsolute", "Absolute url of the content.", resolve: context => context.Source.UrlAbsolute()).SetPermissions(this, true);
            Field<NonNullGraphType<StringGraphType>>("writerName", "Name of the content writer.").SetPermissions(this, true);

            this.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("ancestors")
                .Description("Ancestors of the content.")
                .Argument<BooleanGraphType>("includeSelf", "include self in list")
                .Bidirectional()
                .Resolve(context =>
                    (context.GetArgument<bool?>("includeSelf") == true
                        ? context.Source.AncestorsOrSelf()
                        : context.Source.Ancestors()).Filter(context).ToConnection(context)
                );

            this.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("siblings")
                .Description("Siblings of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Siblings().Filter(context).ToConnection(context));

            this.FilteredConnection<PublishedContentInterfaceGraphType, IPublishedContent>()
                .Name("children")
                .Description("Children of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Children.Filter(context).ToConnection(context));
        }
    }
}
