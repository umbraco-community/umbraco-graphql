using GraphQL;
using GraphQL.Conversion;
using GraphQL.Types;
using Umbraco.Core.Services;
using Our.Umbraco.GraphQL.Types;
using System.Linq;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Types
{
    public class UmbracoSchema : Schema
    {
        public UmbracoSchema(IDependencyResolver dependencyResolver, IContentTypeService contentTypeService, IMediaTypeService mediaTypeService, IPublishedContentGraphTypeBuilder graphTypeBuilder)
        {
            List<IGraphType> contentGraphTypes = graphTypeBuilder.BuildTypes(contentTypeService.GetAll()).ToList();
            List<IGraphType> mediaGraphTypes = graphTypeBuilder.BuildTypes(mediaTypeService.GetAll()).ToList();

            RegisterType(new PublishedContentInterfaceGraphType());
            foreach (var graphType in contentGraphTypes.Concat(mediaGraphTypes))
            {
                RegisterType(graphType);
            }

            DependencyResolver = dependencyResolver;
            FieldNameConverter = new DefaultFieldNameConverter();
            Query = new UmbracoQueryGraphType(contentGraphTypes, mediaGraphTypes);
        }
    }
}
