using System;
using System.Reflection;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters;

namespace Our.Umbraco.GraphQL.Builders
{
    public class SchemaBuilder
    {
        private readonly IGraphTypeAdapter _graphTypeAdapter;

        public SchemaBuilder(IGraphTypeAdapter graphTypeAdapter)
        {
            _graphTypeAdapter = graphTypeAdapter ?? throw new ArgumentNullException(nameof(graphTypeAdapter));
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

            return schema;
        }
    }
}
