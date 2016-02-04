using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Common;
using WGX.Common.Attributes;
using WGX.Lean.DbEntity;

namespace WGX.Lean.BizEntity.Condition
{
    public class LevelCondition : BaseQuery<LenLevel>
    {
        [MapTo("LevelName", IgnoreCase = true, Opt = MapToOpts.Include)]
        public string LevelName
        {
            get;
            set;
        }
    }
}
