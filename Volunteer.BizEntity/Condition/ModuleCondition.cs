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
    public class ModuleCondition : BaseQuery<BaseModule>
    {
        [MapTo("ParentID", IgnoreCase = true, Opt = MapToOpts.Equal)]
        public long? ParentID
        {
            get; set;
        }

        [MapTo("IsBack", IgnoreCase = true, Opt = MapToOpts.Equal)]
        public bool? IsBack
        {
            get;
            set;
        }
    }
}
