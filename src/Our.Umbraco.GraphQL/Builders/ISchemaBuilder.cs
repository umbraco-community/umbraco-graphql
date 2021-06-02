using GraphQL.Types;
using System;

namespace Our.Umbraco.GraphQL.Builders
{
    public interface ISchemaBuilder
    {
        ISchema Build();
    }
}
