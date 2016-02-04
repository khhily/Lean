using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using System.Data.Entity;
using WGX.Lean.BizEntity.Enums;

namespace WGX.Lean.Biz
{
    public class LevelBiz : BaseBiz, ILenLevel
    {
        public IEnumerable<LenLevel> Search()
        {
            return Search(new LevelCondition());
        }

        public IEnumerable<LenLevel> Search(LevelCondition condition)
        {
            using (var db = new Entities())
            {
                IQueryable<LenLevel> query = db.LenLevel;
                query = condition.Filter(query);

                return query.OrderBy(q => q.ID).ToList();
            }
        }

        public LenLevel GetById(long id)
        {
            using (var db = new Entities())
            {
                return db.LenLevel.FirstOrDefault(q => q.ID == id);
            }
        }

        private bool CheckRepeat(LenLevel entity, Entities db)
        {
            if (entity != null)
            {
                return db.LenLevel.Any(q => q.LevelName.ToUpper() == entity.LevelName.ToUpper() && q.ID != entity.ID);
            }
            return false;
        }

        public LenLevel Add(LenLevel entity)
        {
            using (var db = new Entities())
            {
                if (CheckRepeat(entity, db))
                {
                    Errors.Set("Error", "等级名称已经存在!");
                }
                else
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser.ID;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;

                    db.LenLevel.Add(entity);

                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }
                return entity;
            }
        }

        public LenLevel Edit(LenLevel entity)
        {
            using (var db = new Entities())
            {
                if (CheckRepeat(entity, db))
                {
                    Errors.Set("Error", "等级名称已经存在!");
                }
                else
                {
                    var entry = db.Entry(entity);
                    //entry.State = EntityState.Modified;
                    entry.State = EntityState.Unchanged;
                    entry.Property(q => q.LevelName).IsModified = true;
                    entry.Property(q => q.ModifyDate).IsModified = true;
                    entry.Property(q => q.ModifyUserID).IsModified = true;

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }

                return entity;
            }
        }

        public bool Delete(long id)
        {
            using (var db = new Entities())
            {
                var entity = db.LenLevel.FirstOrDefault(q => q.ID == id);
                if (entity != null)
                {
                    if (db.UserInfo.Any(q => q.LevelID == id && q.Status == (int)StatusEnum.Valid))
                    {
                        Errors.Set("Error", "该等级存在用户, 不能删除!");
                    }
                    else
                    {
                        db.LenLevel.Remove(entity);

                        Errors = db.GetErrors();

                        if (!HasError)
                        {
                            db.SaveChanges();
                            return true;
                        }
                    }
                }
                else
                {
                    Errors.Set("not exists", "该等级不存在!");
                }
                return false;
            }
        }
    }
}
