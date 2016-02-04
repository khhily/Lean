using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface IModule : IBaseBiz<BaseModule>
    {
        IEnumerable<BaseModule> Search();

        IEnumerable<BaseModule> Search(ModuleCondition condition);

        IEnumerable<BaseModule> GetModuleByUser(UserInfo user);
    }
}
