using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Transactions;
using Microsoft.Practices.ObjectBuilder2;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using WGX.Lean.DbEntity;
using Microsoft.Practices.Unity;

namespace WGX.Lean.Biz
{
    public class UserRoleBiz : BaseBiz, IUserRole
    {
        [Dependency]
        public ICurrentUser CurrentUserBiz
        {
            get;
            set;
        }
        public IEnumerable<UserRole> Search(UserRoleCondition condition)
        {
            using (var db = new Entities())
            {
                var query = db.UserRole.Where(q => q.Status == (int) StatusEnum.Valid);

                if (condition.ParentID.HasValue)
                {
                    query = query.Where(q => q.ParentID == condition.ParentID.Value);
                }

                if (CurrentUserBiz.CurrentUser != null && CurrentUserBiz.CurrentUser.UserType != (int)UserTypeEnum.SuperAdmin)
                {
                    query = query.Where(q => q.ParentID >= -1);
                }
                return query.OrderByDescending(q => q.ModifyDate).ToList();
            }
        }

        public UserRole GetById(long id)
        {
            using (var db = new Entities())
            {
                return db.UserRole.Include(q => q.UserRoleRight).FirstOrDefault(q => q.ID == id);
            }
        }

        public UserRole Add(UserRole entity)
        {
            using (var db = new Entities())
            {
                if (!CheckUserRole(entity, db))
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser.ID;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;
                    
                    db.UserRole.Add(entity);

                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }
                return entity;
            }
        }

        public UserRole Edit(UserRole entity)
        {
            using (var db = new Entities())
            {
                if (!CheckUserRole(entity, db))
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;

                    var oldObject =
                        db.UserRole.FirstOrDefault(q => q.Status == (int) StatusEnum.Valid && q.ID == entity.ID);
                    if (oldObject != null)
                    {
                        entity.CopyToOnly(oldObject, q => q.RoleName, q => q.Status, q => q.ParentID, q => q.ModifyDate,
                            q => q.ModifyUserID);
                    }

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }
                return entity;
            }
        }

        private bool CheckUserRole(UserRole entity, Entities db)
        {
            if (db.UserRole.Any(q => q.ID != entity.ID && q.RoleName.ToUpper().Trim() == entity.RoleName.ToUpper().Trim() && q.Status == (int) StatusEnum.Valid && q.ID != entity.ID && q.ParentID == entity.ParentID))
            {
                Errors.Set("repeat", "角色名称重复!");
                return true;
            }
            if (!db.UserRole.Any(q => q.ID == entity.ParentID && q.Status == (int)StatusEnum.Valid) && entity.ParentID != 0)
            {
                Errors.Set("notExists", "父角色不存在!");
                return true;
            }
            
            return false;
        }

        public bool Delete(long id)
        {
            using (var db = new Entities())
            {
                var entity = db.UserRole.FirstOrDefault(q => q.ID == id && q.Status == (int) StatusEnum.Valid);
                if (entity != null)
                {
                    if (db.UserInfo.Any(q => q.RoleID == entity.ID))
                    {
                        Errors.Set("ExistsUser", "该角色存在用户信息, 不能删除!");
                        return false;
                    }

                    entity.Status = (int) StatusEnum.Invalid;

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    Errors.Set("Errors", "用户角色不存在!");
                }
                return false;
            }
        }

        public UserRole EditRight(UserRole entity)
        {
            using (var db = new Entities())
            {
                using (var scope = new TransactionScope())
                {
                    var rights = db.UserRoleRight.Where(q => q.RoleID == entity.ID);

                    foreach (var right in rights)
                    {
                        var roleRight = db.UserRoleRight.FirstOrDefault(q => q.ID == right.ID);
                        if (entity.UserRoleRight.All(q => q.ModuleID != right.ModuleID))
                        {
                            #region 删除角色下的用户的相应Module权限
                            var userRights = db.UserRight.Where(q => q.UserInfo.UserRole.ID == right.ID).ToList();
                            //var usrRights = db.UserRight
                            //        .Where(u => u.ModuleID == right.ModuleID &&
                            //                u.UserInfo.UserRight.Any(m => m. == right.RoleID)).ToList();
                            if (userRights.Any())
                            {
                                foreach (var ur in userRights)
                                {
                                    var r = db.UserRight.FirstOrDefault(q => q.ID == ur.ID);
                                    db.UserRight.Remove(r);
                                }
                            }
                            #endregion

                            db.UserRoleRight.Remove(roleRight);
                        }
                    }

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();

                        rights = db.UserRoleRight.Where(q => q.RoleID == entity.ID);
                        entity.UserRoleRight.ForEach(q =>
                        {
                            q.RoleID = entity.ID;
                            q.CreateDate = DateTime.Now;
                            q.CreateUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID; //CurrentUserBiz.CurrentUser.ID;
                            q.ModifyDate = DateTime.Now;
                            q.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID; //CurrentUserBiz.CurrentUser.ID;

                            if (!rights.Any(r => r.ModuleID == q.ModuleID && r.ModuleID > 0))
                            {
                                db.UserRoleRight.Add(q);
                            }
                        });

                        Errors = db.GetErrors();

                        if (!HasError)
                        {
                            db.SaveChanges();
                            scope.Complete();
                        }
                    }
                }

                return entity;
            }
        }
    }
}
