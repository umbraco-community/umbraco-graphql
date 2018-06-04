using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.GraphQL.Types
{
    public class GridControlGraphType : ObjectGraphType<JToken>
    {
        public GridControlGraphType()
        {
            //TODO: Replace with HtmlNodeGraphType?
            Name = "GridControl";
            Field<GridValueGraphType>(
                "value",
                resolve: context =>
                {
                    var userContext = (UmbracoGraphQLContext) context.UserContext;
                    var routeData = new RouteData();
                    routeData.Values["controller"] = "RenderMvc";

                    using (var controller = new RenderMvcController())
                    {
                        var controllerContext = new ControllerContext(userContext.UmbracoContext.HttpContext,
                            routeData,
                            controller);
                        var result = ViewEngines.Engines.FindPartialView(controllerContext, "Grid/Editors/Base");
                        using (var writer = new StringWriter())
                        {
                            var viewData = new ViewDataDictionary(context.Source);
                            result.View.Render(
                                new ViewContext(controllerContext, result.View, viewData,
                                    controllerContext.Controller.TempData, writer), writer);

                            return writer.ToString();
                        }
                    }
                });
            Field<NonNullGraphType<GridEditorGraphType>>(
                "editor",
                resolve: context => context.Source["editor"]
            );
        }
    }
}