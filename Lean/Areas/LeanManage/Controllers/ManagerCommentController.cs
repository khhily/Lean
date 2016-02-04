using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Common.ModelBinder;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;

namespace Lean.Areas.LeanManage.Controllers
{
    public class ManagerCommentController : PrivateController
    {
        [Dependency]
        public IManagerComment ManagerCommentBiz
        {
            get;
            set;
        }
        // GET: LeanManage/ManageComment
        public ActionResult Index(ManagerCommentCondition condition)
        {
            var datas = ManagerCommentBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([ModelBinder(typeof(SmartBinder))][Bind(Include = "Content")]ManagerComment entity)
        {
            if (ModelState.IsValid)
            {
                ManagerCommentBiz.Add(entity);

                if (!ManagerCommentBiz.HasError)
                {
                    SetMessage("保存成功!");
                    return RedirectToAction("Edit", new { id = entity.ID });
                }
                else
                {
                    ParseBizError(ManagerCommentBiz);
                }
            }
            return View(entity);
        }

        public ActionResult Edit(long id)
        {
            var model = ManagerCommentBiz.GetById(id);

            return View("Create", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([ModelBinder(typeof(SmartBinder))][Bind(Include = "ID,Content")]ManagerComment entity)
        {
            if (ModelState.IsValid)
            {
                ManagerCommentBiz.Edit(entity);
                if (!ManagerCommentBiz.HasError)
                {
                    SetMessage("保存成功!");
                }
                else
                {
                    ParseBizError(ManagerCommentBiz);
                }
            }
            return View("Create", entity);
        }

        public ActionResult Delete(long id)
        {
            var msg = "";
            var result = false;
            if (ManagerCommentBiz.Delete(id))
            {
                SetMessage("删除成功!");
                result = true;
            }
            else
            {
                msg = ManagerCommentBiz.Errors.FirstOrDefault().Value;
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}