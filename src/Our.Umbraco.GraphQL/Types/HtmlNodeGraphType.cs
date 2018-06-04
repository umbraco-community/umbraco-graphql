using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Types
{
    public class HtmlNodeGraphType : UnionGraphType
    {
        public HtmlNodeGraphType()
        {
            Name = "HtmlNode";

            Type<HtmlElementGraphType>();
            Type<TextGraphType>();
        }
    }

    public class HtmlElementGraphType : ObjectGraphType
    {
        public HtmlElementGraphType()
        {
            Name = "HtmlElement";

            Field<StringGraphType>("tagName");
            Field<ListGraphType<MetaGraphType>>("attributes");
            Field<ListGraphType<HtmlNodeGraphType>>("children");
        }
    }

    public class TextGraphType : ObjectGraphType
    {
        public TextGraphType()
        {
            Name = "Text";

            Field<StringGraphType>("text");
        }
    }

    public class MetaGraphType : ObjectGraphType
    {
        public MetaGraphType()
        {
            Name = "Meta";

            Field<StringGraphType>("name");
            Field<StringGraphType>("value");
        }
    }
}