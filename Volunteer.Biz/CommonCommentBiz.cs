using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using System.Data.Entity;

namespace WGX.Lean.Biz
{
    public class CommonCommentBiz : BaseBiz, ICommonComment
    {
        public virtual CommonComment GetById(long id)
        {
            using (var db = new Entities())
            {
                if (string.IsNullOrEmpty(ModuleCode.ToString()))
                {
                    return null;
                }
                return db.CommonComment.FirstOrDefault(q => q.ModuleCode.ToUpper() == ModuleCode.ToString().ToUpper());
            }
        }

        public virtual CommonComment GetByCode(string code)
        {
            using (var db = new Entities())
            {
                return db.CommonComment.FirstOrDefault(q => q.ModuleCode.ToUpper() == code.ToString().ToUpper());
            }
        }

        public virtual CommonComment Add(CommonComment entity)
        {
            using (var db = new Entities())
            {
                if (!string.IsNullOrEmpty(ModuleCode.ToString()))
                {
                    if (db.CommonComment.Any(q => q.ModuleCode.ToUpper() == ModuleCode.ToString().ToUpper()))
                    {
                        Errors.Set("Error", "该模块已经有数据!");
                    }
                    else if (entity.DiscussType == (int)DiscussTypeEnum.Edit && string.IsNullOrWhiteSpace(entity.DiscussContent))
                    {
                        Errors.Set("Error", "内容必须填写!");
                    }
                    else if (entity.DiscussType == (int)DiscussTypeEnum.Upload && (string.IsNullOrWhiteSpace(entity.DiscussFileUrl) || string.IsNullOrWhiteSpace(entity.DiscussOfficeFileUrl)))
                    {
                        Errors.Set("Error", "必须上传文件!");
                    }
                    else
                    {
                        var nowDate = DateTime.Now;
                        var userId = CurrentUserBiz.CurrentUser.ID;
                        
                        entity.CreateDate = nowDate;
                        entity.CreateUserID = userId;
                        entity.ModifyDate = nowDate;
                        entity.ModifyUserID = userId;
                        entity.ModuleCode = ModuleCode.ToString();
                        entity.DiscussDate = nowDate;
                        entity.DiscussUserID = userId;

                        db.CommonComment.Add(entity);

                        Errors = db.GetErrors();
                        if (!HasError)
                        {
                            db.SaveChanges();
                        }
                    }
                }
                return entity;
            }
        }

        public virtual CommonComment Edit(CommonComment entity)
        {
            using (var db = new Entities())
            {
                if (db.CommonComment.Any(q => q.ID != entity.ID && q.ModuleCode.ToUpper() == entity.ModuleCode.ToUpper()))
                {
                    Errors.Set("Error", "保存错误!模块代码不符!");
                }
                else
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser.ID;
                    entity.DiscussUserID = CurrentUserBiz.CurrentUser.ID;

                    var entry = db.Entry(entity);
                    entry.State = EntityState.Unchanged;
                    entry.Property(q => q.ModifyDate).IsModified = true;
                    entry.Property(q => q.ModifyUserID).IsModified = true;
                    entry.Property(q => q.DiscussType).IsModified = true;
                    if (entity.DiscussType == (int)DiscussTypeEnum.Edit)
                    {
                        entry.Property(q => q.DiscussContent).IsModified = true;
                    }
                    else
                    {
                        entry.Property(q => q.DiscussFileUrl).IsModified = true;
                        entry.Property(q => q.DiscussOfficeFileUrl).IsModified = true;
                    }
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

        public virtual bool Delete(long id)
        {
            return false;
        }


        private CommonCommentModuleCodeEnum _moduleCode;
        public virtual CommonCommentModuleCodeEnum ModuleCode
        {
            get
            {
                return _moduleCode;
            }
            set
            {
                _moduleCode = value;
            }
        }
    }
}
