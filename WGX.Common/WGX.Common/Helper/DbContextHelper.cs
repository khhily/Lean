using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace WGX.Common.Helper
{
    public static class DbContextHelper
    {
        public static Dictionary<string, string> GetErrors(this System.Data.Entity.DbContext db)
        {
            var datas = new Dictionary<string, string>();
            
            var errs = db.GetValidationErrors().Select(e => e);

            foreach (var e in errs)
            {
                e.ValidationErrors.Select(ee => ee).ToList().ForEach(ee => datas.Set(string.Format("{0} {1}", e.Entry.Entity.GetType(), ee.PropertyName), ee.ErrorMessage));
            }

            return datas;
        }

        public static IEnumerable<T> Query<T>(this System.Data.Entity.DbContext ctx, string query, object parameter)
        {
            var args = AnonymousObjectToParameters(parameter);
// ReSharper disable once CoVariantArrayConversion
            return args != null ? ctx.Database.SqlQuery<T>(query, args) : ctx.Database.SqlQuery<T>(query);
        }

        public static int Execute(this System.Data.Entity.DbContext ctx, string sqlText, object parameter)
        {
            var args = AnonymousObjectToParameters(parameter);
// ReSharper disable once CoVariantArrayConversion
            return args != null ? ctx.Database.ExecuteSqlCommand(sqlText, args) : ctx.Database.ExecuteSqlCommand(sqlText);
        }

        public static T SingleQuery<T>(this System.Data.Entity.DbContext ctx, string query, object parameter)
        {
            return ctx.Query<T>(query, parameter).FirstOrDefault();
        }

        private static SqlParameter[] AnonymousObjectToParameters(object obj)
        {
            if (obj == null)
                return null;

            var ps = obj.GetType().GetProperties();
            return ps.Select(p => new SqlParameter(p.Name, p.GetValue(obj))).ToArray();
        }
    }
}
