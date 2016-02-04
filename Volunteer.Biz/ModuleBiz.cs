using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using WGX.Lean.DbEntity;

namespace WGX.Lean.Biz
{
    public class ModuleBiz : BaseBiz, IModule
    {
        public IEnumerable<BaseModule> Search()
        {
            return Search(new ModuleCondition());
        }

        public IEnumerable<BaseModule> Search(ModuleCondition condition)
        {
            using (var db = new Entities())
            {
                var query = db.BaseModule.Where(q => q.Valid);

                if (condition.ParentID.HasValue)
                {
                    query = query.Where(q => q.ParentID == condition.ParentID.Value);
                }

                if (condition.IsBack.HasValue)
                {
                    query = query.Where(q => q.IsBack == condition.IsBack.Value);
                }

                return query.OrderBy(q => q.ParentID).ThenBy(q => q.ModuleOrder).ToList();
            }
        } 

        public BaseModule GetById(long id)
        {
            using (var db = new Entities())
            {
                return db.BaseModule.FirstOrDefault(q => q.ID == id && q.Valid);
            }
        }

        public BaseModule Add(BaseModule entity)
        {
            using (var db = new Entities())
            {
                if (!CheckModule(entity, db))
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;

                    db.BaseModule.Add(entity);

                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ParseErrors(e);
                        }
                    }
                }

                return entity;
            }
        }

        public BaseModule Edit(BaseModule entity)
        {
            using (var db = new Entities())
            {
                if (!CheckModule(entity, db))
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;

                    var oldEntity = db.BaseModule.FirstOrDefault(q => q.ID == entity.ID && q.Valid);
                    
                    if (oldEntity != null)
                    {
                        entity.CopyToExcept(oldEntity, q => q.ID, q => q.CreateDate, q => q.CreateUserID);

                        if (!HasError)
                        {
                            db.SaveChanges();
                        }
                    }
                }

                return entity;
            }
        }

        public bool Delete(long id)
        {
            using (var db = new Entities())
            {
                using (var scope = new TransactionScope())
                {
                    var module = db.BaseModule.Include(q => q.UserRight).Include(q => q.UserRoleRight).FirstOrDefault(q => q.ID == id && q.Valid);
                    var roleRights = db.UserRoleRight.Where(q => q.ModuleID == id).ToList();
                    var userRights = db.UserRight.Where(q => q.ModuleID == id).ToList();
                    if (module != null)
                    {
                        foreach (var roleRight in roleRights)
                        {
                            var right = db.UserRoleRight.FirstOrDefault(q => q.ID == roleRight.ID);
                            db.UserRoleRight.Remove(right);
                        }

                        foreach (var right in userRights)
                        {
                            var r = db.UserRight.FirstOrDefault(q => q.ID == right.ID);
                            db.UserRight.Remove(r);
                        }

                        module.Valid = false;

                        Errors = db.GetErrors();

                        if (!HasError)
                        {
                            try
                            {
                                db.SaveChanges();
                                scope.Complete();
                            }
                            catch (Exception e)
                            {
                                ParseErrors(e);
                            }
                            if (!HasError)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// 检查模块的数据有效性
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private bool CheckModule(BaseModule entity, Entities db)
        {
            if (db.BaseModule.Any(q => q.ModuleName.Equals(entity.ModuleName, StringComparison.OrdinalIgnoreCase) && q.Valid && q.ID != entity.ID && q.ParentID == entity.ParentID))
            {
                Errors.Set("repeat", String.Format("模块名称{0}已经存在!", entity.ModuleName));
                return true;
            }

            if (db.BaseModule.Any(q => q.ModuleCode.Equals(entity.ModuleCode, StringComparison.OrdinalIgnoreCase) && q.Valid && q.ID != entity.ID && q.ParentID == entity.ParentID))
            {
                Errors.Set("repeat", String.Format("模块代码{0}已经存在!", entity.ModuleCode));
                return true;
            }

            if (entity.ParentID != 0 && !db.BaseModule.Any(q => q.ID == entity.ParentID && q.Valid))
            {
                Errors.Set("not exists", String.Format("父模块不存在!"));
                return true;
            }
            
            return false;
        }

        public IEnumerable<BaseModule> GetModuleByUser(UserInfo user)
        {
            using (var db = new Entities())
            {
                var query = db.BaseModule.Where(q => q.Valid);

                query = (from q in query
                    join u in db.UserRight on q.ID equals u.ModuleID
                    where u.UserID == user.ID
                    select q);

                return query.OrderBy(q => q.ParentID).ThenBy(q => q.ModuleOrder).ToList();
            }
        }
    }
}
