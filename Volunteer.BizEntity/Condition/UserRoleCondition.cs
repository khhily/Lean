using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Common;
using WGX.Lean.DbEntity;

namespace WGX.Lean.BizEntity.Condition
{
    public class UserRoleCondition : BaseQuery<UserRole>
    {
        public long? ParentID
        {
            get;
            set;
        }
    }
}
