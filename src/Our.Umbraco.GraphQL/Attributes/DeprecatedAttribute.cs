using System;

namespace Our.Umbraco.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field |
                    AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property |
                    AttributeTargets.Struct)]
    public class DeprecatedAttribute : Attribute
    {
        public DeprecatedAttribute(string deprecationReason)
        {
            DeprecationReason = deprecationReason;
        }

        public string DeprecationReason { get; }
    }
}
