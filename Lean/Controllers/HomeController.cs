using Lean.Filters;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Common.Enums;
using WGX.Common.Helper;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;

namespace Lean.Controllers
{
    public class HomeController : PublicController
    {
        [Dependency]
        public IManagerComment ManagerCommentBiz
        {
            get;
            set;
        }

        [Dependency]
        public IComment CommentBiz
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

        public ActionResult Index()
        {
            var model = ManagerCommentBiz.GetLatestComment();
            return View(model);
        }

        [HttpPost]
        [NeedLogin(false, Order = 11), CheckPower(false, Order = 12)]
        public ActionResult LeaveMessage(Comment entity)
        {
            var result = false;
            var msg = "";

            if (SessionHelper.Get<UserInfo>(SessionKeys.LoginUser) == null)
            {
                msg = "请先登录!";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entity.Content))
                {
                    msg = "内容不能为空!";
                }
                else
                {
                    CommentBiz.Add(entity);
                    if (!CommentBiz.HasError)
                    {
                        result = true;
                        SetMessage("Operate Success!");
                    }
                    else
                    {
                        msg = CommentBiz.Errors.FirstOrDefault().Value;
                    }
                }
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserCenter(long id)
        {
            var model = UserBiz.GetById(id);
            model.Password = "";
            return View(model);
        }
    }
}