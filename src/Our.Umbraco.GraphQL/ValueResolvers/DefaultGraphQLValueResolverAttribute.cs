using System;

namespace Our.Umbraco.GraphQL.ValueResolvers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class DefaultGraphQLValueResolverAttribute : Attribute
    {
    }
}
