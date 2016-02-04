using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Enums;
using WGX.Common.Helper;
using System.Web;

namespace Lean.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NonPowerAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IModule ModuleBiz
        {
            get;
            set;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller.ControllerContext.IsChildAction)
                return;

            var controller = (string)filterContext.Controller.ControllerContext.RouteData.Values["controller"];
            var action = (string)filterContext.Controller.ControllerContext.RouteData.Values["action"];
            var area = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"];
            var moduleCode = HttpContext.Current.Request.QueryString["ModuleCode"];

            var menus = ModuleBiz.Search(new ModuleCondition {IsBack = false});
            if (menus != null)
            {
                //if (SessionHelper.Get<IEnumerable<BaseModule>>(SessionKeys.FrontMenus) == null)
                //{
                    SessionHelper.Set(SessionKeys.FrontMenus, menus);
                //}
                var baseModules = menus as BaseModule[] ?? menus.ToArray();
                if (baseModules.Any())
                {
                    var url = string.Format("/{0}/{1}/", controller, action);

                    if (area != null && !string.IsNullOrEmpty(area.ToString()))
                        url = string.Format("/{0}/{1}/{2}/", area, controller, action);

                    var menu =baseModules.FirstOrDefault(
                            m => m.ModuleUrl != null && url.StartsWith(m.ModuleUrl.EndsWith("/") ? m.ModuleUrl : (m.ModuleUrl + "/")));

                    if (moduleCode != null)
                    {
                        url = string.Format("/{0}/{1}?ModuleCode={2}", controller, action, moduleCode);
                        menu = baseModules.FirstOrDefault(m => m.ModuleUrl != null && url.StartsWith(m.ModuleUrl));
                    }

                    if (menu != null && menu.ParentID > 0)
                    {
                        menu = baseModules.FirstOrDefault(q => q.ID == menu.ParentID);
                    }
                    filterContext.Controller.TempData["CurrentMenu"] = menu != null
                        ? (object) menu
                        : baseModules.Where(q => q.ModuleUrl == "/Home/Index");
                }
            }
        }

    }
}