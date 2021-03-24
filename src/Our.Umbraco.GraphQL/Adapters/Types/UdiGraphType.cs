using GraphQL.Language.AST;
using GraphQL.Types;
using Umbraco.Cms.Core;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class UdiGraphType : ScalarGraphType
    {
        public UdiGraphType()
        {
            Name = "UDI";
            Description = "Represents an entity identifier.";
        }

        public override object ParseLiteral(IValue value) => value is StringValue stringValue ? ParseValue(stringValue.Value) : null;
        public override object ParseValue(object value) => value is Udi udi ? udi : (UdiParser.TryParse(value?.ToString(), out udi) ? udi : default);
        public override object Serialize(object value) => value is Udi udi ? udi.ToString() : null;
    }
}
