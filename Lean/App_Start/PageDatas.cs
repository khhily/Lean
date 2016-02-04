using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WGX.Common.Enums;
using WGX.Common.Helper;
using EnumHelper = System.Web.Mvc.Html.EnumHelper;
using WGX.Lean.IBiz;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;

namespace Lean
{
    /// <summary>
    /// 在页面上使用,处理一些数据
    /// </summary>
    public class PageDatas
    {
        [Dependency]
        public ICurrentUser CurrentUserBiz
        {
            get;
            set;
        }

        [Dependency]
        public IModule ModuleBiz
        {
            get;
            set;
        }
        [Dependency]
        public IUserRole UserRoleBiz
        {
            get;
            set;
        }
        [Dependency]
        public ILenLevel LenLevelBiz
        {
            get;
            set;
        }


        //private PageDatas() { }

        //private static PageDatas _datas;
        public static PageDatas GetInstance()
        {
            return DependencyResolver.Current.GetService<PageDatas>();
        }
        public IEnumerable<SelectListItem> GetUserTypes(bool super = false)
        {
            var types = GetEnumList(typeof(UserTypeEnum)).ToList();
            super = CurrentUserBiz.CurrentUser != null ? (CurrentUserBiz.CurrentUser.UserType == (int)UserTypeEnum.SuperAdmin) : false;
            if (!super)
            {
                types.RemoveAt(0);
            }
            return types;
        }

        public IEnumerable<SelectListItem> GetUserRoles(bool super = false)
        {
            var roles = UserRoleBiz.Search(new UserRoleCondition()).ToList();
            super = CurrentUserBiz.CurrentUser != null ? (CurrentUserBiz.CurrentUser.UserType == (int)UserTypeEnum.SuperAdmin) : false;
            if (!super)
            {
                var superRole = roles.FirstOrDefault(q => q.ID == 1 && q.RoleName == "SuperManager");
                if (null != superRole)
                {
                    roles.Remove(superRole);
                }
            }
            return roles.Select(q => new SelectListItem { Text = q.RoleName, Value = q.ID.ToString() }).ToList();
        }

        public IEnumerable<SelectListItem> GetModulesSelectList(bool needRoot = false)
        {
            var menus = ModuleBiz.Search().Select(q => new SelectListItem {Text = q.ModuleName, Value = q.ID.ToString()}).ToList();
            if (needRoot)
            {
                menus.Insert(0, new SelectListItem{ Text = "根节点", Value = "0"});
            }
            return menus;
        }


        public IEnumerable<SelectListItem> GetEnumList<T>(T enumType) where T: Type
        {
            return EnumHelper.GetSelectList(enumType);
        }
        
        public IEnumerable<BaseModule> GetModules(object area)
        {
            if (area != null)
            {
                return SessionHelper.Get<IEnumerable<BaseModule>>(SessionKeys.UserMenus) ?? new List<BaseModule>();
            }
            else
            {
                return SessionHelper.Get<IEnumerable<BaseModule>>(SessionKeys.FrontMenus) ?? new List<BaseModule>();
            }
        }

        public IEnumerable<BaseModule> GetModuleFromDB()
        {
            var modules = ModuleBiz.Search().ToList();
            if (CurrentUserBiz.CurrentUser != null && CurrentUserBiz.CurrentUser.UserType != (int)UserTypeEnum.SuperAdmin)
            {
                var moduleManager = modules.FirstOrDefault(q => q.ModuleCode.ToUpper() == "MODULECODE");
                long id = 0;
                if (null != moduleManager)
                {
                    id = moduleManager.ID;
                    modules.Remove(moduleManager);
                }
                var subModules = modules.Where(q => q.ParentID == id).ToList();
                foreach (var item in subModules)
                {
                    var sModule = modules.FirstOrDefault(q => q.ID == item.ID);
                    modules.Remove(sModule);
                }
            }
            
            
            return modules;
        }

        public IEnumerable<SelectListItem> GetUserRoleSelectList(bool needRoot = true)
        {
            var roles = UserRoleBiz.Search(new UserRoleCondition())
                .OrderBy(q => q.ParentID).ThenBy(q => q.RoleName)
                .Select(q => new SelectListItem {Text = q.RoleName, Value = q.ID.ToString()})
                .ToList();
            if (needRoot)
            {
                roles.Insert(0, new SelectListItem
                {
                    Text = "根节点",
                    Value = "0"
                });
            }
            return roles;
        }

        public IEnumerable<SelectListItem> GetLenLevels(bool needRoot = false)
        {
            var levels = new List<SelectListItem>();
            levels = LenLevelBiz.Search().Select(q => new SelectListItem { Text = q.LevelName, Value = q.ID.ToString() }).ToList();
            return levels;
        }
    }
}