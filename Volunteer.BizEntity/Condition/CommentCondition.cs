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
    public class CommentCondition : BaseQuery<Comment>
    {
        [MapTo("UserName", IgnoreCase = true, Opt = MapToOpts.Include)]
        public string UserName { get; set; }

        
        public string CreateDate { get; set; }

        public DateTime? StartCreateDate { get; set; }

        public DateTime? EndCreateDate { get; set; }
    }
}
