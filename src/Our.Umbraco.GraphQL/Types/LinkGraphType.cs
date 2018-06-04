using System;
using GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL.Types
{
    //public class LinkGraphType : ObjectGraphType<Link>
    //{
    //    public LinkGraphType()
    //    {
    //        Name = "Link";

    //        Field<NonNullGraphType<StringGraphType>>("name");
    //        Field<StringGraphType>("target");
    //        Field<NonNullGraphType<LinkTypeGraphType>>("type");
    //        Field<NonNullGraphType<StringGraphType>>("url");
    //        Field<IdGraphType>("id", resolve: context => context.Source.Udi?.ToString());
    //    }
    //}

    //public class LinkTypeGraphType : EnumerationGraphType<LinkType>
    //{
    //    public LinkTypeGraphType()
    //    {
    //        Name = "LinkType";
    //    }
    //}

    //public class MultiUrlPickerValueResolver : GraphQLValueResolver
    //{
    //    public override Type GetGraphQLType(PublishedPropertyType propertyType)
    //    {
    //        return typeof(ListGraphType<LinkGraphType>);
    //    }

    //    public override bool IsConverter(PublishedPropertyType propertyType)
    //    {
    //        return propertyType.PropertyEditorAlias == "RJP.MultiUrlPicker";
    //    }
    //}
}