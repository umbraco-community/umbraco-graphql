using System;
using System.Reflection;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Types;

namespace Our.Umbraco.GraphQL.Builders
{
    public class SchemaBuilder : ISchemaBuilder
    {
        private readonly IGraphTypeAdapter _graphTypeAdapter;
        private readonly IGraphVisitor _visitor;
        private readonly IServiceProvider _serviceProvider;

        public SchemaBuilder(IGraphTypeAdapter graphTypeAdapter, IServiceProvider serviceProvider, IGraphVisitor visitor)
        {
            _graphTypeAdapter = graphTypeAdapter ?? throw new ArgumentNullException(nameof(graphTypeAdapter));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _visitor = visitor;
        }

        public virtual Type SchemaType => typeof(Schema<Query>);

        public virtual ISchema Build()
        {
            var schema = new Schema(_serviceProvider);

            AddProperties(schema);
            _visitor?.Visit(schema);

            return schema;
        }

        protected virtual void AddProperties(Schema schema)
        {
            schema.Query = GenerateFromProperty("Query", true);
        }

        protected virtual IObjectGraphType GenerateFromProperty(string propertyName, bool throwError)
        {
            var queryPropertyInfo = SchemaType.GetProperty(propertyName);
            if (queryPropertyInfo == null)
            {
                if (throwError) throw new ArgumentException($"Could not find property '{propertyName}' on {SchemaType.FullName}.");
                return null;
            }

            if (queryPropertyInfo.CanRead == false)
            {
                if (throwError) throw new ArgumentException($"Could not find getter for '{propertyName}' on {SchemaType.FullName}.");
                return null;
            }

            return (IObjectGraphType)_graphTypeAdapter.Adapt(queryPropertyInfo.GetMethod.ReturnType.GetTypeInfo());
        }
    }
}
