using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Lean.DbEntity
{
    public partial class ManagerComment
    {
        public UserInfo CreateUser
        {
            get;
            set;
        }

        public UserInfo ModifyUser
        {
            get;
            set;
        }
    }
}
