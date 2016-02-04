using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.BizEntity.ViewModels;
using WGX.Lean.IBiz;
using WGX.Common.Encrypte;
using WGX.Common.Helper;
using WGX.Lean.DbEntity;
using System.IO;
using WGX.Site.Common;
using WGX.Site.Common.Enums;

namespace WGX.Lean.Biz
{
    public class UserBiz : BaseBiz, IUser
    {
        public IEnumerable<UserInfo> Search(UserCondition condition)
        {
            using (var db = new Entities())
            {
                var currentUser = CurrentUserBiz.CurrentUser;
                var query = db.UserInfo.Where(q => q.Status == (int) StatusEnum.Valid);

                query = condition.Filter(query);

                if (currentUser != null)
                {
                    query = query.Where(q => q.UserType >= currentUser.UserType);
                }

                return query.OrderBy(q => q.UserCode).DoPage(condition.Pager).ToList();
            }
        }

        public UserInfo Login(LoginUser user)
        {
            using (var db = new Entities())
            {
                var userInfo = db.UserInfo.Include(q => q.UserRole).FirstOrDefault(q => q.UserCode.ToUpper() == user.UserCode.ToUpper());
                if (userInfo != null)
                {
                    var pwd = EncryptionOperate.GetMD5(user.Password, EncryptionOperate.MD5Move);
                    if (userInfo.Password.Equals(pwd, StringComparison.OrdinalIgnoreCase))
                    {
                        return userInfo;
                    }
                    else
                    {
                        Errors.Set("password error", "密码不正确!");
                    }
                }
                else
                {
                    Errors.Set("user not exists", "用户名不存在!");
                }
                return null;
            }
        }

        public UserInfo GetById(long id)
        {
            using (var db = new Entities())
            {
                return db.UserInfo.Include(q => q.UserRight).Include(q => q.UserRole).Include(q => q.LenLevel).FirstOrDefault(q => q.ID == id && q.Status == (int) StatusEnum.Valid);
            }
        }

        public UserInfo Add(UserInfo entity)
        {
            using (var db = new Entities())
            {
                if (CheckUser(entity, db))
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;

                    entity.Password = EncryPassword(entity.Password);

                    db.UserInfo.Add(entity);

                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        db.SaveChanges();
                    }
                }

                return entity;
            }
        }

        public bool CheckUser(UserInfo entity, Entities db)
        {
            var chkUsers = db.UserInfo.Where(q => q.ID != entity.ID && q.Status == (int) StatusEnum.Valid);
            if (chkUsers.Any(q => q.UserCode.ToUpper() == entity.UserCode.ToUpper()))
            {
                Errors.Set("repeat", "用户名已存在!");
                return false;
            }
            if (!db.UserRole.Any(q => q.ID == entity.RoleID))
            {
                Errors.Set("not exists", "用户角色不存在!");
                return false;
            }

            return true;
        }

        public bool CheckUser(UserInfo entity)
        {
            using (var db = new Entities())
            {
                return CheckUser(entity, db);
            }
        }

        public bool ChangePwd(string oldPwd, string newPwd)
        {
            using (var db = new Entities())
            {
                var user = CurrentUserBiz.CurrentUser;
                if (user != null)
                {
                    var userEntity =
                        db.UserInfo.FirstOrDefault(q => q.ID == user.ID && q.Status == (int) StatusEnum.Valid);
                    if (userEntity != null)
                    {
                        var enc = EncryPassword(oldPwd);
                        if (userEntity.Password.ToUpper() != enc.ToUpper())
                        {
                            Errors.Set("Error", "原始密码不正确");
                        }
                        else
                        {
                            userEntity.Password = EncryPassword(newPwd);

                            Errors = db.GetErrors();
                            if (!HasError)
                            {
                                db.SaveChanges();

                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public string EncryPassword(string pwd)
        {
            return !string.IsNullOrEmpty(pwd.Trim()) ? EncryptionOperate.GetMD5(pwd, EncryptionOperate.MD5Move) : "";
        }

        public UserInfo Edit(UserInfo entity)
        {
            using (var db = new Entities())
            {
                using (var scope = new TransactionScope())
                {
                    if (CheckUser(entity, db))
                    {
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;
                        var user =
                            db.UserInfo.FirstOrDefault(q => q.Status == (int) StatusEnum.Valid && q.ID == entity.ID);
                        if (user != null)
                        {
                            //如果更改了角色, 就要修改权限
                            if (user.RoleID != entity.RoleID)
                            {
                                var newRole = db.UserRole.FirstOrDefault(q => q.ID == entity.RoleID);

                                foreach (var right in user.UserRight)
                                {
                                    var userRight = db.UserRight.FirstOrDefault(q => q.ID == right.ID);
                                    //如果现有权限没有在新的角色权限中,删除
                                    if (!newRole.UserRoleRight.Any(q => q.ModuleID == right.ModuleID))
                                    {
                                        db.UserRight.Remove(userRight);
                                    }
                                }

                                Errors = db.GetErrors();

                                if (!HasError)
                                {
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        Errors.Set("Error",
                                            e.InnerException == null ? e.Message : e.InnerException.Message);
                                    }
                                }
                                //
                            }
                            //处理密码
                            if (!string.IsNullOrWhiteSpace(entity.Password))
                            {
                                if (entity.Password.ToUpper().Trim() != user.Password.ToUpper().Trim())
                                {
                                    entity.Password = EncryPassword(entity.Password);
                                }
                            }


                            if (!HasError)
                            {
                                entity.CopyToOnly(user, q => q.IsAdmin, q => q.Email, q => q.ModifyDate,
                                    q => q.ModifyUserID, q => q.QQ, q => q.RoleID, 
                                    q => q.UserCode, q => q.UserGender, q => q.UserName,
                                    q => q.UserType);

                                Errors = db.GetErrors();
                                if (!HasError)
                                {
                                    db.SaveChanges();
                                    scope.Complete();
                                }
                            }
                        }
                        else
                        {
                            Errors.Set("Error", "用户不存在!");
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
                var user = db.UserInfo.FirstOrDefault(q => q.ID == id && q.Status == (int) StatusEnum.Valid);
                if (user != null)
                {
                    user.Status = (int) StatusEnum.Invalid;

                    Errors = db.GetErrors();
                    if (!HasError)
                    {
                        try
                        {
                            db.SaveChanges();

                            return true;
                        }
                        catch (Exception e)
                        {
                            Errors.Set("Error", e.InnerException == null ? e.Message : e.InnerException.Message);
                        }
                    }
                }
                else
                {
                    Errors.Set("Error", "用户不存在");
                }

                return false;
            }
        }

        public UserInfo AssignPermission(UserInfo entity)
        {
            using (var db = new Entities())
            {
                using (var scope = new TransactionScope())
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserID = CurrentUserBiz.CurrentUser == null ? 0 : CurrentUserBiz.CurrentUser.ID;
                    //判断每个Module是否存在于RoleRight中
                    var role = db.UserRole.Include(q => q.UserRoleRight)
                        .FirstOrDefault(q => q.ID == entity.RoleID && q.Status == (int) StatusEnum.Valid);

                    if (role != null)
                    {
                        foreach (var newRight in entity.UserRight)
                        {
                            if (role.UserRoleRight.All(q => q.ModuleID != newRight.ModuleID))
                            {
                                var module = db.BaseModule.FirstOrDefault(q => q.ID == newRight.ModuleID && q.Valid);
                                Errors.Set("Error",
                                    module != null
                                        ? string.Format("角色({0})没有对模块({1})的权限!", role.RoleName, module.ModuleName)
                                        : string.Format("选择的权限超出了角色权限!"));
                            }
                        }

                        if (!HasError)
                        {
                            var oldUserRights = db.UserRight.Where(q => q.UserID == entity.ID).ToList();

                            foreach (var right in oldUserRights)
                            {
                                var userRight = db.UserRight.FirstOrDefault(q => q.ID == right.ID);
                                if (entity.UserRight.All(q => q.ModuleID != right.ModuleID))
                                {
                                    db.UserRight.Remove(userRight);
                                }
                            }
                            Errors = db.GetErrors();
                            if (!HasError)
                            {
                                try
                                {
                                    db.SaveChanges();

                                    var oldRights = db.UserRight.Where(q => q.UserID == entity.ID).ToList();
                                    foreach (var right in entity.UserRight)
                                    {
                                        if (oldRights.All(q => q.ModuleID != right.ModuleID))
                                        {
                                            db.UserRight.Add(new UserRight
                                            {
                                                UserID = entity.ID,
                                                ModuleID = right.ModuleID,
                                                CreateDate = entity.ModifyDate,
                                                CreateUserID = entity.ModifyUserID,
                                                ModifyUserID = entity.ModifyUserID,
                                                ModifyDate = entity.ModifyDate
                                            });
                                        }
                                    }

                                    Errors = db.GetErrors();
                                    if (!HasError)
                                    {
                                        db.SaveChanges();
                                        scope.Complete();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Errors.Set("Error", e.InnerException == null ? e.Message : e.InnerException.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        Errors.Set("Error", "角色不存在!");
                    }
                }
            }
            return entity;
        }

        public bool ChangeImage(long id, string imgUrl)
        {
            using (var db = new Entities())
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var directoryPath = ConfigHelper.UploadFilePath + UploadPathEnum.UserInfo.ToString() + "/";
                if(baseDirectory.EndsWith("/") || baseDirectory.EndsWith("\\"))
                {
                    baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 1);
                }
                baseDirectory = baseDirectory.Replace("/", "\\");
                directoryPath = directoryPath.Replace("/", "\\");
                if (null != imgUrl)
                {
                    if (imgUrl.Trim().StartsWith("/") || imgUrl.Trim().StartsWith("\\"))
                    {
                        imgUrl = imgUrl.Substring(1);
                    }
                    if (!File.Exists(baseDirectory + directoryPath + imgUrl))
                    {
                        Errors.Set("file not exists", "图片未上传成功!");
                        return false;
                    }
                }
                var entity = db.UserInfo.FirstOrDefault(q => q.ID == id && q.Status == (int)StatusEnum.Valid);
                if (null != entity)
                {
                    entity.ImageUrl = imgUrl;

                    Errors = db.GetErrors();

                    if (!HasError)
                    {
                        db.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    Errors.Set("Error", "用户不存在!");
                }
                return false;
            }
        }

        public UserInfo GetOldestUser()
        {
            using (var db = new Entities())
            {
                return db.UserInfo.OrderBy(q => q.CreateDate).FirstOrDefault();
            }
        }
    }
}
