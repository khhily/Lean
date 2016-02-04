using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Common;
using WGX.Lean.DbEntity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;
using System.Data.Entity;

namespace WGX.Lean.Biz
{
    public class CreateModelBiz : BaseBiz, ICreateModel
    {
        public CreateModel GetFirst()
        {
            using (var db = new Entities())
            {
                return db.CreateModel.FirstOrDefault();
            }
        }

        public CreateModel GetById(long id)
        {
            return null;
        }

        public CreateModel Add(CreateModel entity)
        {
            using (var db = new Entities())
            {
                if (db.CreateModel.Any(q => q.CreateUser == entity.CreateUser))
                {
                    return Edit(entity);
                }
                else
                {
                    var regCode = RegistryCodeHelper.GenRegistryCode(entity.CreateUser);
                    if (!string.IsNullOrEmpty(regCode))
                    {
                        if (regCode.ToUpper() == entity.CreateModelCode.ToUpper())
                        {
                            db.CreateModel.Add(entity);

                            Errors = db.GetErrors();

                            if (!HasError)
                            {
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            Errors.Set("Error", "注册码不匹配");
                        }
                    }
                }
                return entity;
            }
        }

        public CreateModel Edit(CreateModel entity)
        {
            using (var db = new Entities())
            {
                if (db.CreateModel.Any(q => q.CreateUser == entity.CreateUser))
                {
                    var entry = db.Entry(entity);

                    entry.State = EntityState.Unchanged;
                    var regCode = RegistryCodeHelper.GenRegistryCode(entity.CreateUser);
                    if (regCode == entity.CreateModelCode)
                    {
                        entry.Property(q => q.CreateModelCode).IsModified = true;

                        Errors = db.GetErrors();
                        if (!HasError)
                        {
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Errors.Set("Error", "注册码不匹配");
                    }
                }

                return entity;
            }
        }

        public bool Delete(long id)
        {
            return false;
        }



        public bool CheckCreateModel(CreateModel model)
        {
            using (var db = new Entities())
            {
                var entity = db.CreateModel.FirstOrDefault(q => q.CreateUser.ToUpper() == model.CreateUser.ToUpper());
                if (null != entity)
                {
                    var regCode = RegistryCodeHelper.GenRegistryCode(model.CreateUser);
                    return regCode.Equals(model.CreateModelCode);
                }
                return true;
            }
        }
    }
}
