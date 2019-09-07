using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class JsonGraphType : StringGraphType
    {
        public JsonGraphType()
        {
            Name = "JSON";
            Description = "The `JSON` scalar type represents JSON values as specified by [ECMA-404](http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf).";
        }
    }
}
