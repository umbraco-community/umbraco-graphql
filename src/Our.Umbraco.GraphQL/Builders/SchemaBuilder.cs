using System;
using System.Reflection;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Visitors;

namespace Our.Umbraco.GraphQL.Builders
{
    public class SchemaBuilder
    {
        private readonly IGraphTypeAdapter _graphTypeAdapter;
        private readonly GraphVisitor _visitor;

        public SchemaBuilder(IGraphTypeAdapter graphTypeAdapter, GraphVisitor visitor)
        {
            _graphTypeAdapter = graphTypeAdapter ?? throw new ArgumentNullException(nameof(graphTypeAdapter));
            _visitor = visitor;
        }

        public ISchema Build<TSchema>() => Build(typeof(TSchema).GetTypeInfo());

        public ISchema Build(TypeInfo schemaType)
        {
            if (schemaType == null) throw new ArgumentNullException(nameof(schemaType));

            var schema = new Schema();

            var queryPropertyInfo = schemaType.GetProperty("Query");
            if(queryPropertyInfo == null)
                throw new ArgumentException($"Could not find property 'Query' on {schemaType}.", nameof(schemaType));

            if(queryPropertyInfo.CanRead == false)
                throw new ArgumentException("'Query' does not have a getter.", nameof(schemaType));
            schema.Query = (IObjectGraphType) _graphTypeAdapter.Adapt(queryPropertyInfo.GetMethod.ReturnType.GetTypeInfo());

            _visitor?.Visit(schema);

            return schema;
        }
    }
}
