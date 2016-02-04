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
    public class CommonCommentCondition : BaseQuery<CommonComment>
    {
        [MapTo("ModuleCode", IgnoreCase = true, Opt = MapToOpts.Equal)]
        public string ModuleCode
        {
            get;
            set;
        }
    }
}
