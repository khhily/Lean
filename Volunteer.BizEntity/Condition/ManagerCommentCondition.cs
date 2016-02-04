using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Common;
using WGX.Lean.DbEntity;

namespace WGX.Lean.BizEntity.Condition
{
    public class ManagerCommentCondition : BaseQuery<ManagerComment>
    {
        public string PublishDate
        {
            get;
            set;
        }

        public DateTime? StartDate
        {
            get;
            set;
        }

        public DateTime? EndDate
        {
            get;
            set;
        }
    }
}
