using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.SystemTextJson;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Examine.Types;
using Our.Umbraco.GraphQL.Adapters.Examine.Visitors;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Types;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Visitors;
using Our.Umbraco.GraphQL.Adapters.Types;
using Our.Umbraco.GraphQL.Adapters.Types.Relay;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Builders;
using Our.Umbraco.GraphQL.Composing;
using Our.Umbraco.GraphQL.Types.PublishedContent;
using Our.Umbraco.GraphQL.Web;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLComposer : ComponentComposer<GraphQLComponent>, IUserComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            base.Compose(builder);

            builder.Services.AddSingleton<ITypeRegistry, TypeRegistry>();
            builder.Services.AddSingleton<IGraphTypeAdapter, GraphTypeAdapter>();
            builder.Services.AddSingleton<ISchemaBuilder, SchemaBuilder>();
            builder.Services.AddSingleton<ISchema, BuiltSchema>();

            builder.GraphVisitors()
                .Append<PublishedContentVisitor>()
                .Append<ExamineVisitor>();

            builder.Services.AddSingleton<IGraphVisitor>(factory => new CompositeGraphVisitor(factory.GetService<GraphVisitorCollection>().ToArray()));

            var serverSection = builder.Config.GetSection("GraphQL:Server");
            var options = serverSection.Get<GraphQLServerOptions>() ?? new GraphQLServerOptions();
            builder.Services
                .AddGraphQL(opts =>
                {
                    opts.ComplexityConfiguration = options.Complexity;
                    opts.EnableMetrics = options.EnableMetrics;
                })
                .AddSystemTextJson()
                .AddDataLoader()
                .AddRelayGraphTypes();

            builder.Services.AddOptions<GraphQLServerOptions>().Bind(serverSection);
            builder.Services.ConfigureOptions<GraphQLUmbracoOptionsSetup>();
            if (options.EnableCors && !string.IsNullOrWhiteSpace(options.CorsPolicyName))
            {
                builder.Services.AddCors(opts =>
                {
                    opts.AddPolicy(options.CorsPolicyName, builder =>
                    {
                        if (options.CorsAllowedExposedHeaders != null && options.CorsAllowedExposedHeaders.Length > 0)
                        {
                            builder.WithExposedHeaders(options.CorsAllowedExposedHeaders);
                        }
                        if (options.CorsAllowedHeaders != null && options.CorsAllowedHeaders.Length > 0)
                        {
                            if (options.CorsAllowedHeaders.Length == 1 && options.CorsAllowedHeaders[0] == "*")
                                builder.AllowAnyHeader();
                            else
                                builder.WithHeaders(options.CorsAllowedHeaders);
                        }
                        if (options.CorsAllowedMethods != null && options.CorsAllowedMethods.Length > 0)
                        {
                            if (options.CorsAllowedMethods.Length == 1 && options.CorsAllowedMethods[0] == "*")
                                builder.AllowAnyMethod();
                            else
                                builder.WithMethods(options.CorsAllowedMethods);
                        }
                        if (options.CorsAllowedOrigins != null && options.CorsAllowedOrigins.Length > 0)
                        {
                            if (options.CorsAllowedOrigins.Length == 1 && options.CorsAllowedOrigins[0] == "*")
                                builder.AllowAnyOrigin();
                            else
                                builder.WithOrigins(options.CorsAllowedOrigins);
                        }
                    });
                });
            }

            // Add all classes that need to be able to be resolved from the service provider
            builder.Services.AddTransient<ExtendQueryWithUmbracoQuery>();
            builder.Services.AddTransient<ExtendUmbracoQueryWithPublishedContentQuery>();
            builder.Services.AddTransient<ExtendUmbracoQueryWithExamineQuery>();
            builder.Services.AddTransient<ExamineQuery>();
            builder.Services.AddTransient<PublishedContentAtRootQuery>();
            builder.Services.AddTransient<PublishedContentByTypeQuery>();
            builder.Services.AddTransient<PublishedContentQuery>();
            builder.Services.AddTransient<UmbracoQuery>();
            builder.Services.AddTransient<PublishedContentTypeGraphType>();
            builder.Services.AddTransient<PublishedElementInterfaceGraphType>();
            builder.Services.AddTransient<PublishedContentInterfaceGraphType>();
            builder.Services.AddTransient<PublishedItemTypeGraphType>();
            builder.Services.AddTransient<ContentVariationGraphType>();
            builder.Services.AddTransient<BlockListItemGraphType>();
            builder.Services.AddTransient<UrlModeGraphType>();
            builder.Services.AddTransient<UdiGraphType>();
            builder.Services.AddTransient<LinkGraphType>();
            builder.Services.AddTransient<LinkTypeGraphType>();
            builder.Services.AddTransient<JsonGraphType>();
            builder.Services.AddTransient<HtmlEncodedStringGraphType>();
            builder.Services.AddTransient<SearchResultsInterfaceGraphType>();
            builder.Services.AddTransient<SearchResultInterfaceGraphType>();
            builder.Services.AddTransient<SearchResultFieldsGraphType>();
            builder.Services.AddTransient<BooleanOperationGraphType>();
            builder.Services.AddTransient<SortDirectionGraphType>();
            builder.Services.AddTransient(typeof(ConnectionGraphType<>));
            builder.Services.AddTransient(typeof(OrderByGraphType<>));
            builder.Services.AddTransient<Adapters.Types.IdGraphType>();

            builder.AddNotificationHandler<ContentTypeChangedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<ContentTypeDeletedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<ContentTypeMovedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<ContentTypeSavedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<DataTypeDeletedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<DataTypeMovedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<DataTypeSavedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MediaTypeChangedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MediaTypeDeletedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MediaTypeMovedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MediaTypeSavedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MemberTypeChangedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MemberTypeDeletedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MemberTypeMovedNotification, SchemaInvalidator>();
            builder.AddNotificationHandler<MemberTypeSavedNotification, SchemaInvalidator>();
        }

        private class SchemaInvalidator :
            INotificationHandler<ContentTypeChangedNotification>,
            INotificationHandler<ContentTypeDeletedNotification>,
            INotificationHandler<ContentTypeMovedNotification>,
            INotificationHandler<ContentTypeSavedNotification>,
            INotificationHandler<DataTypeDeletedNotification>,
            INotificationHandler<DataTypeMovedNotification>,
            INotificationHandler<DataTypeSavedNotification>,
            INotificationHandler<MediaTypeChangedNotification>,
            INotificationHandler<MediaTypeDeletedNotification>,
            INotificationHandler<MediaTypeMovedNotification>,
            INotificationHandler<MediaTypeSavedNotification>,
            INotificationHandler<MemberTypeChangedNotification>,
            INotificationHandler<MemberTypeDeletedNotification>,
            INotificationHandler<MemberTypeMovedNotification>,
            INotificationHandler<MemberTypeSavedNotification>
        {
            private readonly BuiltSchema _schema;

            public SchemaInvalidator(ISchema schema)
            {
                _schema = schema as BuiltSchema;
            }

            public void Handle(ContentTypeChangedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(ContentTypeDeletedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(ContentTypeMovedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(ContentTypeSavedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(DataTypeDeletedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(DataTypeMovedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(DataTypeSavedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MediaTypeChangedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MediaTypeDeletedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MediaTypeMovedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MediaTypeSavedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MemberTypeChangedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MemberTypeDeletedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MemberTypeMovedNotification notification) => _schema?.InvalidateSchema();
            public void Handle(MemberTypeSavedNotification notification) => _schema?.InvalidateSchema();
        }
    }
}
