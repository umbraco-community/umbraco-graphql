using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    public class PublishedContentValueResolver : GraphQLValueResolver
    {
        private readonly IDataTypeService _dataTypeService;

        public PublishedContentValueResolver() : this(ApplicationContext.Current.Services.DataTypeService)
        {
        }

        public PublishedContentValueResolver(IDataTypeService dataTypeService)
        {
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
        }

        public override Type GetGraphQLType(PublishedPropertyType propertyType)
        {
            return IsMultiPicker(propertyType.DataTypeId)
                ? typeof(ListGraphType<PublishedContentGraphType>)
                : typeof(PublishedContentGraphType);
        }

        public override object Resolve(PublishedPropertyType propertyType, object value)
        {
            var isMultiPicker = IsMultiPicker(propertyType.DataTypeId);

            switch (value)
            {
                case IEnumerable<IPublishedContent> contents:
                    var publishedContents = contents;
                    if (isMultiPicker)
                        return publishedContents.ToList();
                    return publishedContents.FirstOrDefault();
                case IPublishedContent content:
                    if (isMultiPicker)
                        return new[] {content};
                    return content;
            }

            return null;
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.ClrType == typeof(IPublishedContent) ||
                   propertyType.ClrType == typeof(IEnumerable<IPublishedContent>);
        }

        protected virtual bool IsMultiPicker(int dataTypeId)
        {
            var preValues = _dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

            if (preValues.PreValuesAsDictionary.TryGetValue("multiPicker", out var preValue))
                return preValue.Value.TryConvertTo<bool>().Result;
            if (preValues.PreValuesAsDictionary.TryGetValue("maxNumber", out preValue) ||
                preValues.PreValuesAsDictionary.TryGetValue("maxItems", out preValue))
            {
                var attempt = preValue.Value.TryConvertTo<int>();
                return false == attempt.Success || attempt.Result != 1;
            }

            return false;
        }
    }
}
