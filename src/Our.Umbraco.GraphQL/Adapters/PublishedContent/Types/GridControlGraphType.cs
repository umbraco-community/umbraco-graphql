using System;
using System.IO;
//using System.Web.Mvc;
//using System.Web.Routing;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using Our.Umbraco.GraphQL.Adapters.Types;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
//using Umbraco.Web.Mvc;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class GridControlGraphType : ObjectGraphType<JToken>
    {
        public GridControlGraphType(IUmbracoContextFactory umbracoContextFactory,
            IPublishedRouter publishedRouter, IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ICompositeViewEngine viewEngine,
            ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            //TODO: Replace with HtmlNodeGraphType?
            Name = "GridControl";
            Field<GridValueGraphType>().Name("value")
                .ResolveAsync(async (context) =>
                {
                    using (var ucRef = umbracoContextFactory.EnsureUmbracoContext())
                    {
                        var umbracoContext = ucRef.UmbracoContext;
                        var routeData = new RouteData();
                        routeData.Values["controller"] = "RenderMvc";

                        using (var controller = (RenderController)serviceProvider.GetService(typeof(RenderController)))
                        {

                            var actionContext = new ActionContext(httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());

                            var result = viewEngine.FindView(actionContext, "Grid/Editors/Base", false);

                            using (var writer = new StringWriter())
                            {

                                var viewData = new ViewDataDictionary<JToken>(
                                    new EmptyModelMetadataProvider(),
                                    new ModelStateDictionary())
                                {
                                    Model = context.Source
                                };
                                var tempData = tempDataDictionaryFactory.GetTempData(httpContextAccessor.HttpContext);
                                await result.View.RenderAsync(
                                    new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
                                         actionContext,
                                         result.View,
                                         viewData,
                                         tempData,
                                         writer,
                                         new HtmlHelperOptions()));

                                return writer.ToString();
                            }

                        }
                    }
                });

            Field<StringGraphType>().Name("rawValue")
                .Description("Raw data value")
                .Resolve((context) =>
                {
                    var source = context.Source["value"];
                    return source.ToString();
                });


            Field<NonNullGraphType<GridEditorGraphType>>().Name("editor")
                .Resolve(context => context.Source["editor"]);

        }
    }
}