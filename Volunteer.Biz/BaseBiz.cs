using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity;
using WGX.Lean.IBiz;
using WGX.Common.Helper;

namespace WGX.Lean.Biz
{
    public class BaseBiz : IBaseBiz
    {
        [Dependency]
        public ICurrentUser CurrentUserBiz
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有错误信息
        /// </summary>
        public bool HasError
        {
            get
            {
                return Errors.Count > 0;
            }
        }

        private Dictionary<string, string> _errors = new Dictionary<string, string>();
        /// <summary>
        /// 错误集合
        /// </summary>
        public Dictionary<string, string> Errors
        {
            get { return _errors ?? (_errors = new Dictionary<string, string>()); }
            set
            {
                _errors = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                var errorMessages = new StringBuilder();
                foreach (var kv in _errors)
                {
                    if (errorMessages.Length > 0)
                    {
                        errorMessages.Append(";" + Environment.NewLine);
                    }
                    errorMessages.AppendFormat("{0}:{1}", kv.Key, kv.Value);
                }

                return errorMessages.ToString();
            }
        }

        public void ParseErrors(Exception e)
        {
            if (e == null) return;

            Errors.Set("Error", e.InnerException != null ? e.InnerException.Message : e.Message);
        }
    }
}
