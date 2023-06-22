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

            Field<PublishedElementInterfaceGraphType>().Name("content")
                .Resolve(context =>
                {

                    // Based on logic in \App_Plugins\DocTypeGridEditor\Render\DocTypeGridEditor.cshtml
                    if (!(context.Source["value"] is JObject))
                    {
                        return null;
                    }

                    // try to load DTGE helper 
                    var docTypeGridEditorHelper = TryGetDocTypeGridEditorHelper(serviceProvider);
                    if (docTypeGridEditorHelper == null)
                    {
                        return null;
                    }

                    string id = context.Source["value"]["id"].ToString();
                    string editorAlias = context.Source["editor"]["alias"].ToString();
                    string contentTypeAlias = "";
                    string value = context.Source["value"]["value"].ToString();
                    //string viewPath = Model.editor.config.viewPath.ToString();

                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            contentTypeAlias = context.Source["value"]["dtgeContentTypeAlias"].ToString();
                        }
                        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                        {
                            contentTypeAlias = context.Source["value"]["docType"].ToString();
                        }

                        if (contentTypeAlias != "")
                        {
                            var content = ConvertValueToContent(docTypeGridEditorHelper, id, contentTypeAlias, value);
                            return content;

                        }
                    }
                    return null;
                }
           );

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



        private static object TryGetDocTypeGridEditorHelper(IServiceProvider serviceProvider)
        {
            try
            {
                var assembly = System.Reflection.Assembly.Load("Our.Umbraco.DocTypeGridEditor");
                var helperType = assembly?.GetType("Our.Umbraco.DocTypeGridEditor.Helpers.DocTypeGridEditorHelper");
                if (helperType == null)
                    return null;
                return serviceProvider.GetService(helperType);
            }
            catch
            {
                return null;
            }
        }

        private static object ConvertValueToContent(object docTypeGridEditorHelper, string id, string contentTypeAlias, string dataJson)
        {
            try
            {
                if (docTypeGridEditorHelper == null)
                {
                    return null;
                }

                var method = docTypeGridEditorHelper.GetType().GetMethod("ConvertValueToContent");
                if (method == null)
                {
                    return null;
                }

                return method.Invoke(docTypeGridEditorHelper, new object[] { id, contentTypeAlias, dataJson });
            }
            catch
            {
                return null;
            }
        }

    }
}