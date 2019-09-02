using System.Reflection;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Builders
{
    public interface ISchemaBuilder
    {
        ISchema Build<TSchema>(params TypeInfo[] additionalTypes);
        ISchema Build(TypeInfo schemaType, params TypeInfo[] additionalTypes);
    }
}
