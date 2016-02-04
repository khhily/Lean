using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Lean.IBiz;
using WGX.Site.Common;

namespace Lean.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CheckRegAttribute : ActionFilterAttribute
    {
        [Dependency]
        public ICreateModel CreateModelBiz
        {
            get;
            set;
        }

        [Dependency]
        public IUser UserBiz
        {
            get;
            set;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var createModel = CreateModelBiz.GetFirst();

            if (CreateModelBiz.GetFirst() != null && CreateModelBiz.CheckCreateModel(createModel))
            {
                return;
            }

            //未通过验证
            var user = UserBiz.GetOldestUser();
            if (null != user && DateTime.Now.Subtract(user.CreateDate).Days <= 30)
            {
                return;
            }

            //未通过验证
            filterContext.Result = new ViewResult { ViewName = "Denied" };
            filterContext.HttpContext.Response.Clear();
        }
    }
}