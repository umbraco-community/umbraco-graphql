using GraphQL.Language.AST;
using Our.Umbraco.GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class IdGraphType : global::GraphQL.Types.IdGraphType
    {
        public override object ParseValue(object value)
        {
            if (value is Id id) return id;
            var parsed = base.ParseValue(value);
            return parsed == null ? (Id?)null : new Id(parsed.ToString());
        }

        public override object ParseLiteral(IValue value)
        {
            var parsed = base.ParseLiteral(value);
            return parsed == null ? (Id?)null : new Id(parsed.ToString());
        }
    }
}
