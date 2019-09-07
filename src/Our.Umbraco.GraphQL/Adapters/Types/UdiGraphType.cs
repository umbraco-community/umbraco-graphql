using GraphQL.Language.AST;
using GraphQL.Types;
using Umbraco.Core;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class UdiGraphType : ScalarGraphType
    {
        public UdiGraphType()
        {
            Name = "Udi";
            Description = "Represents an entity identifier.";
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue stringValue)
                return ParseValue(stringValue.Value);
            return null;
        }

        public override object ParseValue(object value)
        {
            Udi.TryParse(value?.ToString(), out var udi);
            return udi;
        }

        public override object Serialize(object value)
        {
            if (value is Udi udi)
                return udi.ToString();

            return null;
        }
    }
}
