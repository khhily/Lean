using System.Collections;
using System.Collections.Generic;
using WGX.Lean.IBiz;
using WGX.Common.Enums;
using WGX.Common.Helper;
using WGX.Lean.DbEntity;

namespace WGX.Lean.Biz
{
    public class CurrentUserBiz : ICurrentUser
    {
        public UserInfo CurrentUser
        {
            get
            {
                var user = SessionHelper.Get<UserInfo>(SessionKeys.LoginUser);
                return user;
            }
        }
    }
}
