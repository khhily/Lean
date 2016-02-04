using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.Enums;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface IListCommonComment : ICommonComment
    {
        IEnumerable<CommonComment> Search(CommonCommentCondition condition);

        List<string> CodeList { get; set; }
    }
}
