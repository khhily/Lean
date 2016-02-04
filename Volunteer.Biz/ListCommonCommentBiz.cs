using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using WGX.Common;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.BizEntity.Condition;
using WGX.Site.Common;

namespace WGX.Lean.Biz
{
    public class ListCommonCommentBiz : CommonCommentBiz, IListCommonComment
    {
        private List<string> codeList = null;

        public List<string> CodeList
        {
            get
            {
                if (null == codeList)
                {
                    codeList = ConfigHelper.ListCommonComment.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return codeList;
            }
            set
            {
                codeList = value;
            }
        }
        
        public IEnumerable<CommonComment> Search(CommonCommentCondition condition)
        {
            using (var db = new Entities())
            {
                if (!string.IsNullOrEmpty(ModuleCode.ToString().Trim()) && CodeList.Contains(ModuleCode.ToString()))
                {
                    var query = db.CommonComment.Where(q => q.ModuleCode.ToUpper() == ModuleCode.ToString().ToUpper());

                    return query.OrderByDescending(q => q.ModifyDate).DoPage(condition.Pager).ToList();
                }

                return new List<CommonComment>();
            }
        }

        public override CommonComment GetByCode(string code)
        {
            return null;
        }

        public override CommonComment GetById(long id)
        {
            using (var db = new Entities())
            {
                var entity = db.CommonComment.FirstOrDefault(q => q.ID == id);
                if (null != entity && CodeList.Contains(entity.ModuleCode))
                {
                    return entity;
                }
                return null;
            }
        }

        public override CommonComment Add(CommonComment entity)
        {
            using (var db = new Entities())
            {
                if (CheckCommonComment(entity, db) && CodeList.Contains(ModuleCode.ToString()))
                {
                    var date = DateTime.Now;
                    var userId = CurrentUserBiz.CurrentUser.ID;
                    entity.CreateDate = date;
                    entity.CreateUserID = userId;
                    entity.ModifyDate = date;
                    entity.ModifyUserID = userId;
                    entity.DiscussDate = date;
                    entity.DiscussUserID = userId;

                    entity.ModuleCode = ModuleCode.ToString();

                    db.CommonComment.Add(entity);

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                    }

                }
               return entity; 
            }
        }

        public bool CheckCommonComment(CommonComment entity, Entities db)
        {
            if (entity.DiscussType == (int)DiscussTypeEnum.Edit && string.IsNullOrWhiteSpace(entity.DiscussContent))
            {
                Errors.Set("Error", "发布内容不能为空");
            }
            else if (entity.DiscussType == (int)DiscussTypeEnum.Upload && string.IsNullOrWhiteSpace(entity.DiscussFileUrl))
            {
                Errors.Set("Error", "发布文件必须上传!");
            }
            else if(string.IsNullOrWhiteSpace(entity.DiscussTitle))
            {
                Errors.Set("Errors", "标题不能为空!");
            }
            else
            {
                return true;
            }
            return false;
        }

        public override CommonComment Edit(CommonComment entity)
        {
            using (var db = new Entities())
            {
                if (CheckCommonComment(entity, db) && CodeList.Contains(ModuleCode.ToString()))
                {
                    var date = DateTime.Now;
                    var userId = CurrentUserBiz.CurrentUser.ID;

                    entity.ModifyDate = date;
                    entity.ModifyUserID = userId;
                    entity.DiscussDate = date;

                    var entry = db.Entry(entity);

                    entry.State = EntityState.Unchanged;

                    entry.Property(q => q.ModifyDate).IsModified = true;
                    entry.Property(q => q.ModifyUserID).IsModified = true;
                    entry.Property(q => q.DiscussDate).IsModified = true;
                    entry.Property(q => q.DiscussType).IsModified = true;
                    entry.Property(q => q.DiscussTitle).IsModified = true;
                    entry.Property(q => q.DiscussContent).IsModified = true;
                    entry.Property(q => q.DiscussFileUrl).IsModified = true;
                    entry.Property(q => q.DiscussOfficeFileUrl).IsModified = true;
                    entry.Property(q => q.DiscussUserID).IsModified = true;
                    

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                    }

                }
                return entity;
            }
        }

        public override bool Delete(long id)
        {
            using (var db = new Entities())
            {
                if (db.CommonComment.Any(q => q.ID == id))
                {
                    var cur = db.CommonComment.FirstOrDefault(q => q.ID == id);
                    if(cur != null)
                        db.CommonComment.Remove(cur);

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        db.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    Errors.Set("Error", "不存在该记录!");
                }
                return false;
            }
        }
    }
}
