using GraphQL;
using GraphQL.Conversion;
using GraphQL.Instrumentation;
using GraphQL.Introspection;
using GraphQL.Types;
using GraphQL.Utilities;
using Our.Umbraco.GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Our.Umbraco.GraphQL.Builders
{
    public class BuiltSchema : ISchema
    {
        private readonly ISchemaBuilder _builder;
        private Lazy<ISchema> _schema;

        public BuiltSchema(ISchemaBuilder builder)
        {
            _builder = builder;
            _schema = new Lazy<ISchema>(BuildSchema);
        }

        public ExperimentalFeatures Features { get => _schema.Value.Features; set => _schema.Value.Features = value; }
        public bool Initialized => _schema.Value.Initialized;
        public INameConverter NameConverter => _schema.Value.NameConverter;
        public IFieldMiddlewareBuilder FieldMiddleware => _schema.Value.FieldMiddleware;
        public IObjectGraphType Query { get => _schema.Value.Query; set => _schema.Value.Query = value; }
        public IObjectGraphType Mutation { get => _schema.Value.Mutation; set => _schema.Value.Mutation = value; }
        public IObjectGraphType Subscription { get => _schema.Value.Subscription; set => _schema.Value.Subscription = value; }
        public SchemaDirectives Directives => _schema.Value.Directives;
        public SchemaTypes AllTypes => _schema.Value.AllTypes;
        public IEnumerable<Type> AdditionalTypes => _schema.Value.AdditionalTypes;
        public IEnumerable<IGraphType> AdditionalTypeInstances => _schema.Value.AdditionalTypeInstances;
        public IEnumerable<(Type clrType, Type graphType)> TypeMappings => _schema.Value.TypeMappings;
        public IEnumerable<(Type clrType, Type graphType)> BuiltInTypeMappings => _schema.Value.BuiltInTypeMappings;
        public ISchemaFilter Filter { get => _schema.Value.Filter; set => _schema.Value.Filter = value; }
        public ISchemaComparer Comparer { get => _schema.Value.Comparer; set => _schema.Value.Comparer = value; }
        public FieldType SchemaMetaFieldType => _schema.Value.SchemaMetaFieldType;
        public FieldType TypeMetaFieldType => _schema.Value.TypeMetaFieldType;
        public FieldType TypeNameMetaFieldType => _schema.Value.TypeNameMetaFieldType;
        public Dictionary<string, object> Metadata => _schema.Value.Metadata;
        public string Description { get => _schema.Value.Description; set => _schema.Value.Description = value; }

        public TType GetMetadata<TType>(string key, TType defaultValue = default) => _schema.Value.GetMetadata(key, defaultValue);
        public TType GetMetadata<TType>(string key, Func<TType> defaultValueFactory) => _schema.Value.GetMetadata(key, defaultValueFactory);
        public bool HasMetadata(string key) => _schema.Value.HasMetadata(key);
        public void Initialize() => _schema.Value.Initialize();
        public void RegisterType(IGraphType type) => _schema.Value.RegisterType(type);
        public void RegisterType(Type type) => _schema.Value.RegisterType(type);
        public void RegisterTypeMapping(Type clrType, Type graphType) => _schema.Value.RegisterTypeMapping(clrType, graphType);
        public void RegisterVisitor(ISchemaNodeVisitor visitor) => _schema.Value.RegisterVisitor(visitor);
        public void RegisterVisitor(Type type) => _schema.Value.RegisterVisitor(type);

        public void InvalidateSchema()
        {
            if (!_schema.IsValueCreated) return;
            _schema = new Lazy<ISchema>(BuildSchema);
        }

        private ISchema BuildSchema() => _builder.Build(typeof(Schema<Query, Mutation>).GetTypeInfo());
    }
}
