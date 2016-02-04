using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Lean.BizEntity.Enums
{
    public enum UserTypeEnum
    {
        /// <summary>
        /// 超级管理员, 拥有所有权限
        /// </summary>
        SuperAdmin = -1,
        /// <summary>
        /// 管理员,拥有后台权限
        /// </summary>
        Admin = 1,
        /// <summary>
        /// 普通会员,拥有前端权限
        /// </summary>
        Member = 2
    }
}
