using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using System.IO;
using WGX.Site.Common;
using System.Diagnostics;
using Lean.Filters;

namespace Lean.Areas.LeanManage.Controllers
{
    public class CommonCommentController : PrivateController
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

        public ActionResult Index(CommonCommentCondition condition)
        {
            if (string.IsNullOrWhiteSpace(condition.ModuleCode))
            {
                condition.ModuleCode = "None";
            }
            CommonCommentBiz.ModuleCode = condition.ModuleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            if (CommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None && ListCommonCommentBiz.CodeList.Contains(CommonCommentBiz.ModuleCode.ToString()))
            {
                ListCommonCommentBiz.ModuleCode = CommonCommentBiz.ModuleCode;
                var datas = ListCommonCommentBiz.Search(condition);
                var model = PDM.Create(datas, condition);
                return View(model);
            }
            return RedirectToAction("Create", new { moduleCode = condition.ModuleCode });
        }

        public ActionResult Create(string moduleCode = "None")
        {
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                moduleCode = "None";
            }
            CommonCommentBiz.ModuleCode = moduleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            CommonComment entity = null;
            if (CommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None)
            {
                //如果是非列表模块, 则要到数据库取值
                if (!ListCommonCommentBiz.CodeList.Contains(CommonCommentBiz.ModuleCode.ToString()))
                {
                    entity = CommonCommentBiz.GetByCode(moduleCode);
                }

                if (null == entity)
                {
                    entity = new CommonComment { ModuleCode = moduleCode };
                }
            }
            else
            {
                SetMessage("模块代码错误!");
                entity = new CommonComment();
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CommonComment entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ModuleCode))
            {
                entity.ModuleCode = "None";
            }
            CommonCommentBiz.ModuleCode = entity.ModuleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            if (CommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None)
            {
                if (!ListCommonCommentBiz.CodeList.Contains(entity.ModuleCode))
                {
                    if (entity.ID == 0)
                    {
                        CommonCommentBiz.Add(entity);
                    }
                    else
                    {
                        CommonCommentBiz.Edit(entity);
                    }
                }
                else
                {
                    ListCommonCommentBiz.ModuleCode = CommonCommentBiz.ModuleCode;
                    if (entity.ID == 0)
                    {
                        ListCommonCommentBiz.Add(entity);
                    }
                    else
                    {
                        ListCommonCommentBiz.Edit(entity);
                    }
                }

                if (!CommonCommentBiz.HasError && !ListCommonCommentBiz.HasError)
                {
                    SetMessage("保存成功!");
                    if (entity.ID > 0)
                    {
                        entity = CommonCommentBiz.GetByCode(entity.ModuleCode);
                    }
                }
                else
                {
                    ParseBizError(CommonCommentBiz);
                    ParseBizError(ListCommonCommentBiz);
                }
            }
            else
            {
                SetMessage("模块代码错误!");
            }
            return View(entity);
        }

        public ActionResult Edit(long id, string moduleCode = "None")
        {
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                moduleCode = "None";
            }
            ListCommonCommentBiz.ModuleCode = moduleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            CommonComment entity = null;
            if (ListCommonCommentBiz.ModuleCode != CommonCommentModuleCodeEnum.None && ListCommonCommentBiz.CodeList.Contains(ListCommonCommentBiz.ModuleCode.ToString()))
            {
                entity = ListCommonCommentBiz.GetById(id);
                if (null == entity || (null != entity && entity.ModuleCode != ListCommonCommentBiz.ModuleCode.ToString()))
                {
                    SetMessage("模块代码错误!");
                    entity = new CommonComment();
                }
                if (null == entity)
                {
                    entity = new CommonComment() { ModuleCode = moduleCode };
                }
            }
            else
            {
                SetMessage("模块代码错误!");
                entity = new CommonComment();
            }
            return View("Create", entity);
        }

        public ActionResult Delete(long id, string moduleCode = "None")
        {
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                moduleCode = "None";
            }
            var msg = "";
            var result = false;
            CommonCommentBiz.ModuleCode = moduleCode.ToEnum(CommonCommentModuleCodeEnum.None);
            var model = ListCommonCommentBiz.GetById(id);
            if (model != null && model.ModuleCode == CommonCommentBiz.ModuleCode.ToString())
            {
                if (ListCommonCommentBiz.Delete(id))
                {
                    result = true;
                    SetMessage("删除成功!");
                }
                else
                {
                    msg = ListCommonCommentBiz.Errors.FirstOrDefault().Value;
                }
            }
            else
            {
                msg = "模块代码错误!";
            }

            return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);

        }

        [CheckPower(false, Order= 12)]
        public ActionResult UploadOfficeFile(string ModuleCode = "None")
        {
            var msg = "";
            var result = false;
            var officeUrl = "";
            var flashUrl = "";
            var p2fPath = ConfigHelper.P2FServerPath;
            if (string.IsNullOrWhiteSpace(ModuleCode))
            {
                ModuleCode = "None";
            }
            CommonCommentBiz.ModuleCode = ModuleCode.ToEnum(CommonCommentModuleCodeEnum.None);

            if (CommonCommentBiz.ModuleCode == CommonCommentModuleCodeEnum.None)
            {
                msg = "模块代码错误!";
            }
            else
            {
                if (Request.Files.Count > 0 && Request.Files[0] != null)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    if (basePath.Trim().EndsWith("\\"))
                    {
                        basePath = basePath.Substring(0, basePath.Length - 1);
                    }
                    string uploadFilePath = ConfigHelper.UploadFilePath;
                    string swfFilePath = ConfigHelper.ConvertSwfPath;
                    string saveOfficeFilePath = basePath + uploadFilePath;
                    string saveSwfFilePath = basePath + swfFilePath;

                    saveOfficeFilePath = saveOfficeFilePath.Replace("/", "\\");
                    saveSwfFilePath = saveSwfFilePath.Replace("/", "\\");

                    if (!Directory.Exists(saveOfficeFilePath))
                    {
                        Directory.CreateDirectory(saveOfficeFilePath);
                    }

                    if (!Directory.Exists(saveSwfFilePath))
                    {
                        Directory.CreateDirectory(saveSwfFilePath);
                    }

                    string fileName = Request.Files[0].FileName;
                    
                    string tmpName = fileName.Substring(0, fileName.LastIndexOf("."));
                    string officeExt = Path.GetExtension(fileName);
                    int index = 1;
                    while (System.IO.File.Exists(saveOfficeFilePath + tmpName + officeExt))
                    {
                        tmpName = fileName.Substring(0, fileName.LastIndexOf(".")) + "(" + index.ToString() + ")";
                        index++;
                    }

                    string officeFileUrl = uploadFilePath + tmpName + officeExt;

                    Request.Files[0].SaveAs(saveOfficeFilePath + tmpName + officeExt);

                    string swfTmpname = tmpName;
                    //swfTmpname = swfTmpname.Substring(0, swfTmpname.LastIndexOf("."));
                    string ext = ".swf";
                    index = 1;
                    while (System.IO.File.Exists(saveSwfFilePath + swfTmpname + ext))
                    {
                        swfTmpname = swfTmpname + "(" + index.ToString() + ")";
                        index++;
                    }

                    Process.Start(p2fPath, string.Format("\"{0}\" \"{1}\"", saveOfficeFilePath + tmpName + officeExt, saveSwfFilePath + swfTmpname + ext));

                    officeUrl = tmpName + officeExt;
                    flashUrl = swfTmpname + ext;
                    result = true;
                }
                else
                {
                    msg = "请选择要上传的文件!";
                }
            }

            return Json(new { success = result, message = msg, flashUrl = flashUrl, officeUrl = officeUrl }, JsonRequestBehavior.AllowGet);
        }
    }
}