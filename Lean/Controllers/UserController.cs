using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WGX.Common.Enums;
using WGX.Common.Helper;
using WGX.Common.ModelBinder;
using WGX.Lean.IBiz;
using Lean.Filters;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.ViewModels;


namespace Lean
{
    public class UserController : PrivateController
    {
        [Dependency]
        public IUser UserBiz
        {
            get;
            set;
        }

        [Dependency]
        public IModule ModuleBiz
        {
            get;
            set;
        }

        [NeedLogin(false, Order = 11), CheckPower(false, Order = 12)]
        public ActionResult Login()
        {
            if (SessionHelper.Get<UserInfo>(SessionKeys.LoginUser) != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [NeedLogin(false, Order = 11), CheckPower(false, Order = 12)]
        public ActionResult Login([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "ConfirmPwd, NewPassword, OldPassword")]LoginUser user)
        {
            var userInfo = UserBiz.Login(user);
            var result = false;
            var msg = "";
            long id = 0;
            bool isAdmin = false;
            if (null != userInfo)
            {
                SessionHelper.Set(SessionKeys.LoginUser, userInfo);
                var modules = ModuleBiz.GetModuleByUser(userInfo);
                if (modules != null)
                {
                    SessionHelper.Set(SessionKeys.UserMenus, modules);
                }
                result = true;
                msg = userInfo.UserName;
                isAdmin = userInfo.IsAdmin;
                id = userInfo.ID;
            }
            else
            {
                msg = UserBiz.Errors.Count > 0 ? UserBiz.Errors.FirstOrDefault().Value : "Login Failure!";
            }

            return Json(new { success = result, message = msg, id = id, IsAdmin = isAdmin }, JsonRequestBehavior.AllowGet);
        }

        [NeedLogin(Order = 11), CheckPower(false, Order = 12)]
        public ActionResult Logout()
        {
            bool result = false;
            if (SessionHelper.Remove(SessionKeys.LoginUser))
            {
                result = true;
            }
            return Json(new {success = result}, JsonRequestBehavior.AllowGet);
        }

        [CheckPower(false, Order = 11)]
        public ActionResult ChangePwd()
        {
            //var entity = CurrentUser;
            return View("ChangePassword");
        }

        [CheckPower(false, Order = 11)]
        [HttpPost]
        public ActionResult ChangePwd([ModelBinder(typeof(SmartBinder))][Bind(Include = "ConfirmPwd, NewPassword, OldPassword")]LoginUser entity)
        {
            var msg = "";
            var res = false;
            if (ModelState.IsValid)
            {
                res = UserBiz.ChangePwd(entity.OldPassword, entity.NewPassword);
                if (res)
                {
                    SetMessage("修改密码成功!");
                }
                else
                {
                    msg = UserBiz.Errors.FirstOrDefault().Value;
                }
            }
            return Json(new {success = res, message = msg}, "text/html", JsonRequestBehavior.AllowGet);
        }

        
    }
}
