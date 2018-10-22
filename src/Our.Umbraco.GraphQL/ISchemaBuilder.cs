using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;
using System;
using Umbraco.Core.Services;

namespace Our.Umbraco.GraphQL
{
    public interface ISchemaBuilder
    {
        ISchema Build();
    }

    public class UmbracoSchemaBuilder : ISchemaBuilder
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IPublishedContentGraphTypeBuilder _graphTypeBuilder;
        private readonly IMediaTypeService _mediaTypeService;

        public UmbracoSchemaBuilder(IContentTypeService contentTypeService, IMediaTypeService mediaTypeService, IPublishedContentGraphTypeBuilder graphTypeBuilder, IDependencyResolver dependencyResolver)
        {
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _graphTypeBuilder = graphTypeBuilder ?? throw new ArgumentNullException(nameof(graphTypeBuilder));
            _mediaTypeService = mediaTypeService ?? throw new ArgumentNullException(nameof(mediaTypeService));
        }

        public ISchema Build()
        {
            return new UmbracoSchema(_dependencyResolver, _contentTypeService, _mediaTypeService, _graphTypeBuilder);
        }
    }
}
