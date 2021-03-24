using GraphQL;
using GraphQL.Http;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Builders;
using Umbraco.Cms.Core.Composing;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Composing;
using System.Linq;
using Our.Umbraco.GraphQL.Web;
using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Adapters.PublishedContent.Visitors;
using GraphQL.DataLoader;
using Our.Umbraco.GraphQL.Adapters.Examine.Visitors;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Our.Umbraco.GraphQL.Compose
{
    public class GraphQLComposer : ComponentComposer<GraphQLComponent>, IUserComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            base.Compose(builder);

            builder.Services.AddSingleton<ITypeRegistry, TypeRegistry>();
            builder.Services.AddScoped<IGraphTypeAdapter, GraphTypeAdapter>();
            builder.Services.AddScoped<ISchemaBuilder, SchemaBuilder>();

            builder.GraphVisitors()
                .Append<PublishedContentVisitor>()
                .Append<ExamineVisitor>();

            builder.Services.AddSingleton(new GraphQLRequestParser(new JsonSerializer()));
            builder.Services.AddScoped<IGraphVisitor>(factory => new CompositeGraphVisitor(factory.GetService<GraphVisitorCollection>().ToArray()));

            builder.Services.AddSingleton<IDependencyResolver>(factory =>
                new FuncDependencyResolver(type => factory.GetService(type) ?? factory.CreateInstance(type)));
            builder.Services.AddSingleton<IDocumentWriter, DocumentWriter>();

            builder.Services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            builder.Services.AddSingleton<DataLoaderDocumentListener>();
        }
    }
}
