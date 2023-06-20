using System.IO;
//using System.Web.Mvc;
//using System.Web.Routing;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types;
//using Umbraco.Web.Mvc;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridControlGraphType : ObjectGraphType<JToken>
    {
        public GridControlGraphType()
        {
            //TODO: Replace with HtmlNodeGraphType?
            Name = "GridControl";
            Field<StringGraphType>(
                "value",
                resolve: (context) => {
                    var source = context.Source["value"];
                    return source.ToString();
                }
            );
            
            Field<NonNullGraphType<GridEditorGraphType>>(
                "editor",
                resolve: context => context.Source["editor"]
            );
        }
    }
}