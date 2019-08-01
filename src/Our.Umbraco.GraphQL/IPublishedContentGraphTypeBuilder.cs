using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.ValueResolvers;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.GraphQL
{
    public interface IPublishedContentGraphTypeBuilder
    {
        IEnumerable<IGraphType> BuildTypes(IEnumerable<IContentTypeComposition> contentTypes);
        IEnumerable<FieldType> BuildContentPropertyFieldTypes(IContentTypeComposition contentType);
    }

    public class PublishedContentGraphTypeBuilder : IPublishedContentGraphTypeBuilder
    {
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
        private readonly GraphQLValueResolverCollection _graphQLValueResolvers;

        public PublishedContentGraphTypeBuilder(IPublishedContentTypeFactory publishedContentTypeFactory, GraphQLValueResolverCollection graphQLValueResolvers)
        {
            _publishedContentTypeFactory = publishedContentTypeFactory ?? throw new System.ArgumentNullException(nameof(publishedContentTypeFactory));
            _graphQLValueResolvers = graphQLValueResolvers ?? throw new System.ArgumentNullException(nameof(graphQLValueResolvers));
        }

        public IEnumerable<IGraphType> BuildTypes(IEnumerable<IContentTypeComposition> contentTypes)
        {
            List<IContentTypeComposition> contentTypeList = contentTypes.ToList();
            List<IContentTypeComposition> compositions = contentTypeList.SelectMany(x => x.ContentTypeComposition).Distinct().ToList();
            
            var interfaceGraphTypes = new Dictionary<string, IInterfaceGraphType>();

            foreach (IContentTypeComposition contentType in compositions)
            {
                PublishedContentCompositionGraphType graphType = new PublishedContentCompositionGraphType(contentType, this);
                yield return graphType;
                interfaceGraphTypes.Add(contentType.Alias, graphType);
            }

            foreach (IContentTypeComposition contentType in contentTypes.Except(compositions))
            {
                PublishedContentGraphType graphType = new PublishedContentGraphType(contentType, interfaceGraphTypes, this);
                yield return graphType;
            }
        }

        public IEnumerable<FieldType> BuildContentPropertyFieldTypes(IContentTypeComposition contentType)
        {
            IPublishedContentType publishedContentType = _publishedContentTypeFactory.CreateContentType(contentType);
            foreach (PropertyType property in contentType.CompositionPropertyTypes)
            {
                yield return new PublishedPropertyFieldType(publishedContentType, property, _graphQLValueResolvers);
            }
        }
    }
}
