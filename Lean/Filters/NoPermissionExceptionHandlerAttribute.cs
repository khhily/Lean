using Lean;
using System.Web.Mvc;

namespace Lean.Filters
{
    public class NoPermissionExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is NoPermissionException)
            {
                var result = new ViewResult
                {
                    ViewName = "Denied",
                    TempData = filterContext.Controller.TempData
                };
                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}