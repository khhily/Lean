using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface ILenLevel : IBaseBiz<LenLevel>
    {
        IEnumerable<LenLevel> Search();

        IEnumerable<LenLevel> Search(LevelCondition condition);
    }
}
