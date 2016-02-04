using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;
using WGX.Common.Enums;
using WGX.Common.Helper;
using Microsoft.Practices.Unity;
using WGX.Lean.IBiz;
using WGX.Lean.BizEntity.Condition;
using System.Web;

namespace Lean.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CheckPowerAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IModule ModuleBiz
        {
            get;
            set;
        }

        private bool _check;

        public CheckPowerAttribute(bool check = true)
        {
            _check = check;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var attrs = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).OfType<NeedLoginAttribute>().ToList();
            attrs.AddRange(filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<NeedLoginAttribute>());
            var needLogin = attrs.OrderByDescending(a => a.Order).FirstOrDefault();

            if (needLogin != null && !needLogin.Need)
                _check = false;

            if (_check)
            {
                if (filterContext.Controller.ControllerContext.IsChildAction)
                    return;

                var controller = (string)filterContext.Controller.ControllerContext.RouteData.Values["controller"];
                var action = (string)filterContext.Controller.ControllerContext.RouteData.Values["action"];
                var area = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"];
                var code = HttpContext.Current.Request.QueryString["ModuleCode"]??HttpContext.Current.Request["ModuleCode"];

                var menus = SessionHelper.Get<IEnumerable<BaseModule>>(SessionKeys.UserMenus);

                if (menus != null)
                {
                    var baseModules = menus as BaseModule[] ?? menus.ToArray();

                    var url = string.Format("/{0}/{1}/", controller, action);

                    if (area != null && !string.IsNullOrEmpty(area.ToString()))
                        url = string.Format("/{0}/{1}/{2}/", area, controller, action);

                    if (controller.ToUpper() == "COMMONCOMMENT" && code != null && !string.IsNullOrEmpty(code.ToString()))
                    {
                        if (area != null && !string.IsNullOrEmpty(code.ToString()))
                        {
                            url = string.Format("/{0}/{1}/Index?ModuleCode={2}/", area, controller, code);
                        }
                        else
                        {
                            url = string.Format("/{0}/Index?ModuleCode={1}/", controller, code);
                        }
                    }

                    var menu = baseModules.FirstOrDefault(
                            m => m.ModuleUrl != null && url.StartsWith(m.ModuleUrl.EndsWith("/") ? m.ModuleUrl : (m.ModuleUrl + "/")));

                    if (menu != null)
                    {
                        if (!(controller.ToUpper() == "COMMONCOMMENT" && string.IsNullOrEmpty(code)))
                        {
                            if (menu.ParentID > 0)
                            {
                                menu = menus.FirstOrDefault(q => q.ID == menu.ParentID);
                            }

                            filterContext.Controller.TempData["CurrentMenu"] = menu;
                            filterContext.Controller.ViewData["CurrentMenu"] = menu;
                            //OK, 有权限
                            return;
                        }
                    }
                    if (SessionHelper.Get<UserInfo>(SessionKeys.LoginUser).UserType == UserTypeEnum.SuperAdmin.GetEnumValue())
                    {
                        //var userMenu = new BaseModule()
                        //{
                        //    ID = 0,
                        //    ModuleCode = "",
                        //    ModuleUrl = url
                        //};
                        //filterContext.Controller.TempData["CurrentMenu"] = userMenu;
                        //有权限
                        return;
                    }
                }

                //没有权限
                filterContext.Controller.TempData["CurrentMenu"] = null;
                filterContext.Result = new ViewResult
                {
                    ViewName = "Denied",
                    TempData = filterContext.Controller.TempData
                };
                filterContext.HttpContext.Response.Clear();
            }
        }

    }
}