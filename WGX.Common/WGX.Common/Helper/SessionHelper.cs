using System.Web;
using System.Web.SessionState;
using WGX.Common.Enums;

namespace WGX.Common.Helper
{
    public static class SessionHelper
    {
        public static T Get<T>(SessionKeys key)
        {
            HttpSessionState session = HttpContext.Current.Session;
            if (session != null)
            {
                return (T)session[key.ToString()];
            }
            return default(T);
        }

        public static bool Set<T>(SessionKeys key, T obj)
        {
            var session = HttpContext.Current.Session;
            if (session != null)
            {
                session[key.ToString()] = obj;
                return true;
            }
            return false;
        }

        public static bool Remove(SessionKeys key)
        {
            var session = HttpContext.Current.Session;
            if (session != null)
            {
                session.Remove(key.ToString());
                return true;
            }
            return false;
        }
    }
}
