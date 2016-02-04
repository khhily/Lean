using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WGX.Common.Enums;
using WGX.Common.Helper;
using WGX.Common.ModelBinder;
using Lean.Filters;
using WGX.Lean.IBiz;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.Enums;

namespace Lean.Areas.LeanManage.Controllers
{
    public class ModuleController : PrivateController
    {
        [Dependency]
        public IModule ModuleBiz
        {
            get;
            set;
        }

        public ActionResult Index()
        {
            var datas = ModuleBiz.Search();
            var model = PDM.Create(datas, new ModuleCondition());
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ModuleCondition condition)
        {
            var datas = ModuleBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        public ActionResult Create(int id = 0)
        {
            return View(new BaseModule {ParentID = id, Valid = true});
        }

        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "ID,CreateUserID, ModifyUserID")]BaseModule entity)
        {
            if (ModelState.IsValid)
            {
                ModuleBiz.Add(entity);
                if (!ModuleBiz.HasError)
                {
                    SetMessage("保存成功!");
                }
                else
                {
                    ParseBizError(ModuleBiz);
                }
            }
            return View(entity);
        }

        public ActionResult Edit(int id)
        {
            var model = ModuleBiz.GetById(id);
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "CreateUserID, ModifyUserID")]BaseModule entity)
        {
            if (ModelState.IsValid)
            {
                ModuleBiz.Edit(entity);

                if (!ModuleBiz.HasError)
                {
                    SetMessage("保存成功!");
                }
                else
                {
                    ParseBizError(ModuleBiz);
                }
            }
            return View("Create", entity);
        }

        public ActionResult Delete(int id)
        {
            // ReSharper disable once RedundantAssignment
            var res = new { success = false, message = "" };

            res = ModuleBiz.Delete(id) ? new {success = true, message = "删除成功"} : new {success = false, message = ModuleBiz.HasError ? ModuleBiz.Errors.FirstOrDefault().Value : "删除失败"};
            SetMessage(res.message);
            return Json(res, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubMenus(long id = 0)
        {
            var datas = ModuleBiz.Search(new ModuleCondition{ ParentID = id }).Where(q => q.ParentID == id).Select(q =>
                new
                {
                    id = q.ID,
                    q.ModuleCode,
                    q.ModuleName,
                    q.ModuleUrl,
                    pid = q.ParentID,
                    q.IsBack,
                    q.IsMenu,
                    q.ModuleOrder
                });

            return Json(new {success = true, Menus = datas}, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}