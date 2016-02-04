using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface ICommonComment : IBaseBiz<CommonComment>
    {
        CommonCommentModuleCodeEnum ModuleCode { get; set; }

        CommonComment GetByCode(string code);
    }
}
