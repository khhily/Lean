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

namespace WGX.Lean.Biz
{
    public class CommentBiz : BaseBiz, IComment
    {
        public IEnumerable<Comment> Search(CommentCondition condition)
        {
            using (var db = new Entities())
            {
                IQueryable<Comment> query = db.Comment.Include(m => m.UserInfo).Include(m => m.UserRole);

                query = condition.Filter(query);
                if (condition.StartCreateDate.HasValue)
                {
                    var date = condition.StartCreateDate.Value.Date;
                    query = query.Where(q => q.CreateDate >= date);
                }

                if (condition.EndCreateDate.HasValue)
                {
                    var date = condition.EndCreateDate.Value.Date.AddDays(1).AddMilliseconds(-1);
                    query = query.Where(q => q.CreateDate <= date);
                }

                return query.OrderByDescending(q => q.CreateDate).DoPage(condition.Pager).ToList();
            }
        }

        public Comment GetById(long id)
        {
            using (var db = new Entities())
            {
                var entity = db.Comment.FirstOrDefault(q => q.ID == id);
                return entity;
            }
        }

        public Comment Add(Comment entity)
        {
            using (var db = new Entities())
            {
                if (CurrentUserBiz.CurrentUser != null)
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser.ID;
                    entity.ReplyID = 0;
                    entity.UserID = CurrentUserBiz.CurrentUser.ID;
                    entity.UserName = CurrentUserBiz.CurrentUser.UserName;
                    entity.RoleID = CurrentUserBiz.CurrentUser.RoleID;
                    entity.RoleName = CurrentUserBiz.CurrentUser.UserRole.RoleName;

                    db.Comment.Add(entity);
                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }
                else
                {
                    Errors.Set("Error", "请先登录!");
                }

                return entity;
            }
        }

        public Comment Edit(Comment entity)
        {
            using (var db = new Entities())
            {
                if (entity.ID > 0)
                {
                    var entry = db.Entry<Comment>(entity);
                    entry.State = EntityState.Unchanged;
                    entry.Property(q => q.Content).IsModified = true;

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
                var entity = db.Comment.FirstOrDefault(q => q.ID == id);

                if (entity != null)
                {
                    db.Comment.Remove(entity);

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
    }
}
