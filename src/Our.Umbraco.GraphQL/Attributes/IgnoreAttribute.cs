using System;

namespace Our.Umbraco.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
