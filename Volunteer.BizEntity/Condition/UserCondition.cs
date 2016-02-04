using System.ComponentModel.DataAnnotations;
using WGX.Common;
using WGX.Common.Attributes;
using WGX.Lean.DbEntity;

namespace WGX.Lean.BizEntity.Condition
{
    public class UserCondition : BaseQuery<UserInfo>
    {
        [MapTo("UserName", IgnoreCase = true, Opt = MapToOpts.Include)]
        [Display(Name = "用户姓名")]
        public string UserName
        {
            get; set;
        }

        [Display(Name = "用户代码")]
        [MapTo("UserCode", IgnoreCase = true, Opt = MapToOpts.Include)]
        public string UserCode
        {
            get;
            set;
        }
    }
}
