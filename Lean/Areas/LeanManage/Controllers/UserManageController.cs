using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WGX.Common.ModelBinder;
using Lean.Filters;
using WGX.Lean.IBiz;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.Enums;

namespace Lean.Areas.LeanManage.Controllers
{
    public class UserManageController : PrivateController
    {
        [Dependency]
        public IUser UserBiz
        {
            get;
            set;
        }
        
        public ActionResult Index()
        {
            var condition = new UserCondition();
            var datas = UserBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UserCondition condition)
        {
            var datas = UserBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        public ActionResult Create()
        {
            return View(new UserInfo{ Status = (int)StatusEnum.Valid});
        }

        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(SmartBinder))][Bind(Exclude="ID,CreateDate,CreateUserID,ModifyDate,ModifyUserID")]UserInfo entity)
        {
            if (ModelState.IsValid)
            {
                UserBiz.Add(entity);
                SetMessage(!UserBiz.HasError ? "保存成功!" : UserBiz.Errors.FirstOrDefault().Value);
            }
            return View(entity);
        }

        public ActionResult Edit(long id)
        {
            var entity = UserBiz.GetById(id);
            return View("Create", entity);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "CreateDate,CreateUserID,ModifyDate,ModifyUserID")]UserInfo entity)
        {
            if (ModelState.IsValid)
            {
                UserBiz.Edit(entity);
                SetMessage(!UserBiz.HasError ? "保存成功!" : UserBiz.Errors.FirstOrDefault().Value);
            }
            return View("Create", entity);
        }

        public ActionResult Delete(long id)
        {
            var res = false;
            var msg = "";
            if (UserBiz.Delete(id))
            {
                res = true;
                //msg = "删除成功!";
                SetMessage("删除成功!");
            }
            else
            {
                msg = UserBiz.Errors.FirstOrDefault().Value;
            }
            return Json(new {success = res, message = msg}, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignPermission(long id)
        {
            var model = UserBiz.GetById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignPermission(UserInfo entity)
        {
            UserBiz.AssignPermission(entity);
            SetMessage(UserBiz.HasError ? UserBiz.Errors.FirstOrDefault().Value : "授权成功!");
            return View(entity);
        }

        public ActionResult UserImageManage(long id)
        {
            var model = UserBiz.GetById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult UserImageManage([ModelBinder(typeof(SmartBinder))][Bind(Include = "ID,ImageUrl")]UserInfo entity)
        {
            var msg = "";
            var result = false;

            if (ModelState.IsValid)
            {
                if (UserBiz.ChangeImage(entity.ID, entity.ImageUrl))
                {
                    SetMessage("保存成功!");
                    result = true;
                }
                else
                {
                    msg = UserBiz.Errors.FirstOrDefault().Value;
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "保存失败!";
                    }
                }
            }

            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImageFile()
        {
            return base.UploadFiles(".jpg,.png,.gif,.bmp");
        }
    }
}