using System.Web;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.AspNetCore.Html;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class HtmlGraphType : ScalarGraphType
    {
        public HtmlGraphType()
        {
            Name = "HTML";
            Description = "A string containing HTML code.";
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue stringValue)
                return ParseValue(stringValue.Value);
            return null;
        }

        public override object ParseValue(object value)
        {
            if(value is string stringValue)
                return new HtmlString(stringValue);

            return null;
        }

        public override object Serialize(object value)
        {
            if (value is HtmlString htmlString)
                return htmlString.ToString();

            return null;
        }
    }
}
