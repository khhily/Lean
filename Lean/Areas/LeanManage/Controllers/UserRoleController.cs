using System.Data;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Web.Mvc;
using WGX.Common.ModelBinder;
using Lean.Filters;
using WGX.Lean.IBiz;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.Enums;

namespace Lean.Areas.LeanManage.Controllers
{
    public class UserRoleController : PrivateController
    {
        [Dependency]
        public IUserRole UserRoleBiz
        {
            get;
            set;
        }

        public ActionResult Index()
        {
            var condition = new UserRoleCondition();
            var datas = UserRoleBiz.Search(condition).ToList();
            if (CurrentUser.UserType != (int)UserTypeEnum.SuperAdmin)
            {
                var sr = datas.FirstOrDefault(q => q.RoleName == "SuperManager");
                if (sr != null)
                {
                    datas.Remove(sr);
                }
            }
            var model = PDM.Create(datas.AsEnumerable(), condition);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UserRoleCondition condition)
        {
            var datas = UserRoleBiz.Search(condition);
            var model = PDM.Create(datas, condition);
            return View(model);
        }

        public ActionResult GetSubRoles(long id = 0)
        {
            var condition = new UserRoleCondition { ParentID = id};
            var datas = UserRoleBiz.Search(condition).Select(q => new
                            {
                                id = q.ID,
                                pid = q.ParentID,
                                q.RoleName,
                                CreatedDate = q.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                ModifyDate = q.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss")
                            });

            return Json(new {success = true, data = datas}, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(long id = 0)
        {
            return View(new UserRole {ParentID = id, Status = (int) StatusEnum.Valid});
        }

        [HttpPost]
        public ActionResult Create([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "ID,CreateUserID, ModifyUserID")]UserRole entity)
        {
            if (ModelState.IsValid)
            {
                UserRoleBiz.Add(entity);

                SetMessage(!UserRoleBiz.HasError ? "保存成功!" : UserRoleBiz.Errors.FirstOrDefault().Value);
            }
            return View(entity);
        }

        public ActionResult Edit(long id)
        {
            var model = UserRoleBiz.GetById(id);
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(SmartBinder))][Bind(Exclude = "CreateUserID, ModifyUserID")]UserRole entity)
        {
            if (ModelState.IsValid)
            {
                UserRoleBiz.Edit(entity);
                SetMessage(UserRoleBiz.HasError ? UserRoleBiz.Errors.FirstOrDefault().Value : "保存成功!");
            }
            return View("Create", entity);
        }

        public ActionResult Delete(long id)
        {
            var succ = false;
            string msg;
            if (id > 0)
            {
                succ = UserRoleBiz.Delete(id);
                msg = UserRoleBiz.HasError ? UserRoleBiz.Errors.FirstOrDefault().Value : "删除成功!";
            }
            else
            {
                msg = "ID不存在!";
            }
            return Json(new {success = succ, message = msg}, "text/html", JsonRequestBehavior.AllowGet);
        }


        public ActionResult RoleAuthorize(long id)
        {
            var entity = UserRoleBiz.GetById(id);
            return View(entity);
        }

        [HttpPost]
        public ActionResult RoleAuthorize(UserRole entity)
        {
            if (entity.UserRoleRight.Count > 0 && !entity.UserRoleRight.Any(q => q.ModuleID <= 0))
            {
                UserRoleBiz.EditRight(entity);

                SetMessage(!UserRoleBiz.HasError ? "授权成功!" : UserRoleBiz.Errors.FirstOrDefault().Value);
            }
            return View(entity);
        }
    }
}