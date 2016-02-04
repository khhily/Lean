using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WGX.Lean.BizEntity.ViewModels
{
    public class LoginUser
    {
        [DisplayName("用户名")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string UserCode
        {
            get; set;
        }

        [DisplayName("旧密码")]
        [Required(ErrorMessage = "{0} 必须填写")]
        public string OldPassword
        {
            get;
            set;
        }

        [DisplayName("密码")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Password
        {
            get; set;
        }

        [DisplayName("新密码")]
        [Required(ErrorMessage = "{0} 必须填写")]
        public string NewPassword
        {
            get;
            set;
        }

        [DisplayName("重复密码")]
        [Compare("NewPassword", ErrorMessage = "两次密码输出必须相同")]
        public string ConfirmPwd
        {
            get;
            set;
        }

        [DisplayName("验证码")]
        public string CaptchaCode
        {
            get;
            set;
        }
    }
}
