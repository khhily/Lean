using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Lean.BizEntity.Enums
{
    public enum StatusEnum
    {
        [Description("有效")]
        Valid = 1,

        [Description("无效")]
        Invalid = 0
    }
}
