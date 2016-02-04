using Lean.Filters;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;

namespace Lean.Areas.LeanManage.Controllers
{
    public class ManageHomeController : PrivateController
    {
        [Dependency]
        public ICreateModel CreateModelBiz
        {
            get;
            set;
        }

        // GET: VolunteerManage/Home
        public ActionResult Index()
        {
            return View();
        }

        [CheckPower(false, Order = 11)]
        public ActionResult Registry()
        {
            var model = CreateModelBiz.GetFirst();
            return View(model);
        }

        [CheckPower(false, Order = 11)]
        [HttpPost]
        public ActionResult Registry(CreateModel entity)
        {
            if (ModelState.IsValid)
            {
                if (CreateModelBiz.CheckCreateModel(entity))
                {
                    if (CreateModelBiz.GetFirst() == null)
                    {
                        CreateModelBiz.Add(entity);
                    }
                    else
                    {
                        CreateModelBiz.Edit(entity);
                    }
                    if (!CreateModelBiz.HasError)
                    {
                        SetMessage("注册成功!");
                    }
                    else
                    {
                        ParseBizError(CreateModelBiz);
                    }
                }
                else
                {
                    SetMessage("验证码不匹配");
                }
            }
            return View(entity);
        }
    }
}