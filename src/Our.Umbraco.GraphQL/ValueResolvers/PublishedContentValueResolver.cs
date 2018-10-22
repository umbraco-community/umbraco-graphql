using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [DefaultGraphQLValueResolver]
    public class PublishedContentValueResolver : GraphQLValueResolver
    {
        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return IsMultiPicker(propertyType.DataType)
                ? typeof(ListGraphType<PublishedContentInterfaceGraphType>)
                : typeof(PublishedContentInterfaceGraphType);
        }

        public override bool IsResolver(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(IPublishedContent) ||
                   propertyType.ClrType == typeof(IEnumerable<IPublishedContent>);
        }

        public override object Resolve(IPublishedElement owner, PublishedPropertyType propertyType, object value)
        {
            var isMultiPicker = IsMultiPicker(propertyType.DataType);

            switch (value)
            {
                case IEnumerable<IPublishedContent> contents:
                    var publishedContents = contents;
                    if (isMultiPicker)
                        return publishedContents.ToList();
                    return publishedContents.FirstOrDefault();
                case IPublishedContent content:
                    if (isMultiPicker)
                        return new[] { content };
                    return content;
                default:
                    return null;
            }
        }

        protected virtual bool IsMultiPicker(PublishedDataType dataType)
        {
            var configuration = dataType.Configuration;

            if(configuration is MultiNodePickerConfiguration mnpc)
            {
                return mnpc.MaxNumber != 1;
            }
            if(configuration is NestedContentConfiguration ncc)
            {
                return ncc.MaxItems != 1;
            }
            if(configuration is MediaPickerConfiguration mpc)
            {
                return mpc.Multiple;
            }
            // TODO: Check other types

            return false;
        }
    }
}
