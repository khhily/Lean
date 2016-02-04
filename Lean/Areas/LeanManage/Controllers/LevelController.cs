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
    public class LevelController : PrivateController
    {
        [Dependency]
        public ILenLevel LevelBiz
        {
            get;
            set;
        }

        // GET: LeanManage/Level
        public ActionResult Index(LevelCondition condition)
        {
            var datas = LevelBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(SmartBinder))][Bind(Include = "LevelName")]LenLevel entity)
        {
            var msg = "";
            var result = false;
            if (ModelState.IsValid)
            {
                LevelBiz.Add(entity);

                if (!LevelBiz.HasError)
                {
                    //msg = "保存成功!";
                    SetMessage("保存成功!");
                    result = true;
                }
                else
                {
                    msg = LevelBiz.Errors.FirstOrDefault().Value;
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "保存失败!";
                    }
                }
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(long id)
        {
            var model = LevelBiz.GetById(id);

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(SmartBinder))][Bind(Include = "ID,LevelName")]LenLevel entity)
        {
            var msg = "";
            var result = false;
            if (ModelState.IsValid)
            {
                LevelBiz.Edit(entity);
                if (!LevelBiz.HasError)
                {
                    //msg = "保存成功!";
                    SetMessage("保存成功!");
                    result = true;
                }
                else
                {
                    msg = LevelBiz.Errors.FirstOrDefault().Value;
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "保存失败!";
                    }
                }
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id)
        {
            var msg = "";
            var result = false;
            if (LevelBiz.Delete(id))
            {
                //msg = "删除成功!";
                SetMessage("删除成功!");
                result = true;
            }
            else
            {
                msg = LevelBiz.Errors.FirstOrDefault().Value;
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "删除失败!";
                }
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}