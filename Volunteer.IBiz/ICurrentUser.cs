using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface ICurrentUser
    {
        UserInfo CurrentUser
        {
            get;
        }
    }
}
