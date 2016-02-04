using WGX.Lean.Biz;
using WGX.Lean.DbEntity;
using Lean.Filters;
using WGX.Lean.IBiz;

namespace Lean
{
    [NeedLogin(Order = 2), CheckPower(Order = 3)]
    public class PrivateController : BaseController
    {
        private ICurrentUser _currentUserBiz;
        private ICurrentUser CurrentUserBiz
        {
            get
            {
                return _currentUserBiz ?? (_currentUserBiz = new CurrentUserBiz());
            }
        }

        public UserInfo CurrentUser
        {
            get
            {
                return CurrentUserBiz.CurrentUser;
            }
        }
    }
}