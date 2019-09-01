using System;

namespace Our.Umbraco.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter |
                    AttributeTargets.Property)]
    public class NonNullItemAttribute : Attribute
    {
    }
}
