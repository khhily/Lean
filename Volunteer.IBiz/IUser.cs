using System.Collections.Generic;
using WGX.Lean.BizEntity.Condition;
using WGX.Lean.BizEntity.ViewModels;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface IUser : IBaseBiz<UserInfo>
    {
        IEnumerable<UserInfo> Search(UserCondition condition);

        UserInfo Login(LoginUser user);

        bool CheckUser(UserInfo entity);

        UserInfo AssignPermission(UserInfo entity);

        bool ChangePwd(string oldPwd, string newPwd);

        bool ChangeImage(long id, string imgUrl);

        UserInfo GetOldestUser();
    }
}
