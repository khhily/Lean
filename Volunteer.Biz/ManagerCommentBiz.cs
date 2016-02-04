using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.IBiz;
using WGX.Lean.DbEntity;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Common;
using WGX.Common.Helper;
using System.Data.Entity;

namespace WGX.Lean.Biz
{
    public class ManagerCommentBiz : BaseBiz, IManagerComment
    {

        public IEnumerable<ManagerComment> Search(ManagerCommentCondition condition)
        {
            using (var db = new Entities())
            {
                var id = CurrentUserBiz.CurrentUser.ID;
                IQueryable<ManagerComment> query = db.ManagerComment.Include(q => q.UserInfo).Where(q => q.UserID == id);

                if (condition.StartDate.HasValue)
                {
                    var date = condition.StartDate.Value.Date;
                    query = query.Where(q => q.ModifyDate >= date);
                }
                if (condition.EndDate.HasValue)
                {
                    var date = condition.EndDate.Value.Date.AddDays(1).AddMilliseconds(-1);
                    query = query.Where(q => q.ModifyDate <= date);
                }

                var result = query.OrderByDescending(q => q.ModifyDate).DoPage(condition.Pager).ToList();

                foreach (var item in result)
                {
                    item.CreateUser = db.UserInfo.FirstOrDefault(q => q.Status == (int)StatusEnum.Valid && q.ID == item.CreateUserID);
                    item.ModifyUser = db.UserInfo.FirstOrDefault(q => q.Status == (int)StatusEnum.Valid && q.ID == item.ModifyUserID);
                }

                return result;
            }
        }

        public ManagerComment GetById(long id)
        {
            using (var db = new Entities())
            {
                var userId = CurrentUserBiz.CurrentUser.ID;
                return db.ManagerComment.FirstOrDefault(q => q.ID == id && q.UserID == userId);
            }
        }

        public ManagerComment Add(ManagerComment entity)
        {
            using (var db = new Entities())
            {
                if (!string.IsNullOrWhiteSpace(entity.Content))
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser.ID;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;

                    entity.UserID = CurrentUserBiz.CurrentUser.ID;
                    entity.UserName = CurrentUserBiz.CurrentUser.UserName;

                    db.ManagerComment.Add(entity);
                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }
                return entity;
            }
        }

        public ManagerComment Edit(ManagerComment entity)
        {
            using (var db = new Entities())
            {
                if (!string.IsNullOrWhiteSpace(entity.Content))
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;

                    entity.UserID = CurrentUserBiz.CurrentUser.ID;
                    entity.UserName = CurrentUserBiz.CurrentUser.UserName;

                    var entry = db.Entry(entity);
                    entry.State = EntityState.Unchanged;
                    entry.Property(q => q.Content).IsModified = true;
                    entry.Property(q => q.ModifyDate).IsModified = true;
                    entry.Property(q => q.ModifyUserID).IsModified = true;
                    entry.Property(q => q.UserID).IsModified = true;
                    entry.Property(q => q.UserName).IsModified = true;

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
                var entity = db.ManagerComment.FirstOrDefault(q => q.ID == id);
                if (null != entity)
                {
                    db.ManagerComment.Remove(entity);

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();

                        return true;
                    }
                }
                return false;
            }
        }

        public ManagerComment GetLatestComment()
        {
            using (var db = new Entities())
            {
                var entity = db.ManagerComment.OrderByDescending(q => q.ModifyDate).FirstOrDefault();
                return entity;
            }
        }
    }
}
