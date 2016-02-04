using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface IUserRole : IBaseBiz<UserRole>
    {
        IEnumerable<UserRole> Search(UserRoleCondition condition);

        UserRole EditRight(UserRole entity);
    }
}
