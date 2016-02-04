using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.BizEntity.Condition;

namespace Lean.Controllers
{
    public class FrontCommonCommentController : PublicController
    {
        [Dependency]
        public ICommonComment CommonCommentBiz
        {
            get;
            set;
        }

        [Dependency]
        public IListCommonComment ListCommonCommentBiz
        {
            get;
            set;
        }

        // GET: FrontCommonComment
        public ActionResult Index(CommonCommentCondition condition)
        {
            if (string.IsNullOrWhiteSpace(condition.ModuleCode))
            {
                condition.ModuleCode = "None";
            }
            CommonCommentBiz.ModuleCode = condition.ModuleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            if (CommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None)
            {
                if (ListCommonCommentBiz.CodeList.Contains(condition.ModuleCode))
                {
                    ListCommonCommentBiz.ModuleCode = CommonCommentBiz.ModuleCode;
                    //返回集合
                    var datas = ListCommonCommentBiz.Search(condition);
                    var model = PDM.Create(datas, condition);
                    return View(model);
                }
                else
                {
                    var model = CommonCommentBiz.GetByCode(condition.ModuleCode);

                    return View("Create", model);
                }
            }
            return View("Denied");
        }

        public ActionResult Detail(long id, string ModuleCode = "None")
        {
            if (string.IsNullOrWhiteSpace(ModuleCode))
            {
                ModuleCode = "None";
            }
            ListCommonCommentBiz.ModuleCode = ModuleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            if (ListCommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None)
            {
                var model = ListCommonCommentBiz.GetById(id);
                if (ListCommonCommentBiz.ModuleCode.ToString() == model.ModuleCode)
                {
                    return View("Create", model);
                }
            }
            return View("Denied");
        }
    }
}