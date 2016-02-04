using System;
using System.Web.Mvc;
using System.Web.Routing;
using WGX.Lean.DbEntity;
using WGX.Common.Enums;
using WGX.Common.Helper;

namespace Lean.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NeedLoginAttribute : ActionFilterAttribute {

        public bool Need;

        public NeedLoginAttribute(bool need = true) {
            Need = need;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {

            if (filterContext.Controller.ControllerContext.IsChildAction)
                return;

            if (Need)
            {
                var user = SessionHelper.Get<UserInfo>(SessionKeys.LoginUser);
                if (user == null) {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            area = "",
                            action = "Login",
                            controller = "User",
                            url = filterContext.HttpContext.Request.Url
                        }));
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}