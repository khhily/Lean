using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Lean.BizEntity.Enums
{
    public enum DiscussTypeEnum
    {
        [Description("编辑发布")]
        Edit = 1,
        [Description("上传文件发布")]
        Upload = 2
    }
}
