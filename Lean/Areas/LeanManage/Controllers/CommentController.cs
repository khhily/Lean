using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.IBiz;

namespace Lean.Areas.LeanManage.Controllers
{
    public class CommentController : PrivateController
    {
        [Dependency]
        public IComment CommentBiz
        {
            get;
            set;
        }
        // GET: LeanManage/Comment
        public ActionResult Index(CommentCondition condition)
        {
            var datas = CommentBiz.Search(condition);
            var model = PDM.Create(datas.AsEnumerable(), condition);
            return View(model);
        }

        public ActionResult Delete(long id)
        {
            var result = false;
            var msg = "";
            if (CommentBiz.Delete(id))
            {
                SetMessage("删除成功!");
                result = true;
            }
            else
            {
                msg = CommentBiz.Errors.FirstOrDefault().Value;
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "删除失败!";
                }
            }
            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}