using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WGX.Common;

namespace Lean.Filters
{
    public class ExceptionLogAttribute : FilterAttribute, IExceptionFilter
    {

        [Dependency]
        public ILog Log
        {
            get;
            set;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {

                HandleErrorInfo model;
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];

                if (filterContext.Exception is HttpRequestValidationException)
                {

                    if (filterContext.IsChildAction)
                    {
                        ViewContext par = filterContext.ParentActionViewContext;
                        while (null != par)
                        {
                            var wtr = (StringWriter)par.Writer;
                            wtr.GetStringBuilder().Clear();
                            par = par.ParentActionViewContext;
                        }
                    }

                    model = new HandleErrorInfo(new Exception("非法字符串"), controllerName, actionName);
                }
                else
                {
                    var ex = filterContext.Exception.GetBaseException();
                    Log.Log(ex);

                    model = new HandleErrorInfo(ex, controllerName, actionName);
                }


                var result = new ViewResult
                {
                    ViewName = "Exception",
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}