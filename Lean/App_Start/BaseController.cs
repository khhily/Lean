using System.Linq;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using Lean.Filters;
using WGX.Lean.IBiz;
using System.IO;
using System;

// ReSharper disable once CheckNamespace
namespace Lean
{
    [ExceptionLog(Order = 1)]
    public class BaseController : Controller
    {
        [NonAction]
        public void SetMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;

            TempData["ShowMessage"] = msg;
        }

        [NonAction]
        public void SetModelValidationError()
        {
            if (!ModelState.IsValid)
            {
                var msg = ModelState.Values.FirstOrDefault(q => q.Errors.Count > 0).Errors.FirstOrDefault().ErrorMessage;
                TempData["ShowMessage"] = msg;
            }
        }

        [NonAction]
        public void ParseBizError(IBaseBiz biz)
        {
            if (biz.HasError)
            {
                //biz.Errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));
                TempData["ShowMessage"] = biz.Errors.FirstOrDefault().Value;
            }
        }

        [NeedLogin(true, Order = 11), CheckPower(false, Order = 12)]
        public ActionResult UploadFiles(string fileExts = ".jpg,.gif,.png")
        {
            var msg = "";
            var result = false;
            var fileUrl = "";
            var imgUrl = "";
            if (Request.Files.Count <= 0)
            {
                msg = "未上传文件";
            }
            else 
            {
                var file = Request.Files[0];
                if(file != null)
                {
                    var fileName = file.FileName;
                    var ext = Path.GetExtension(fileName);
                    var exts = fileExts.Split(',');
                    if (!exts.Contains(ext))
                    {
                        msg = string.Format("无效的文件类型! 只支持{0}类型的文件", fileExts);
                    }
                    else
                    {
                        imgUrl = "/UploadFile/UserInfo/";
                        var basePath = Server.MapPath(imgUrl);
                        if(!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        var tmpFileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        int index = 0;
                        while (System.IO.File.Exists(basePath + tmpFileName + ext))
                        {
                            tmpFileName = string.Format("{0}({1})",fileName, index);
                        }
                        fileName = tmpFileName + ext;
                        try
                        {
                            file.SaveAs(basePath + fileName);
                            fileUrl = fileName;
                            imgUrl += fileName;
                            msg = "文件上传成功!";
                            result = true;
                        }
                        catch (Exception e)
                        {
                            msg = "上传文件错误!";
                        }   
                    }
                }
                else
                {
                    msg = "未发现文件";
                }
            }
            return Json(new { success = result, message = msg, fileurl = fileUrl, imgUrl = imgUrl }, JsonRequestBehavior.AllowGet);
        }
    }
}