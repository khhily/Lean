using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace WGX.Common.Encrypte
{
    public static class EncryptionOperate
    {
        public static readonly string MD5Move = "WGX_Volunteer";
        /// <summary>
        /// 加密成32位16进制字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HashMD5_String(string str)
        {
            if (String.IsNullOrEmpty(str)) return "";

            // ReSharper disable once CSharpWarnings::CS0618
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
        
        /// <summary>
        /// 增加偏移量,生成32位16进制字符串
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static string GetMD5(string sDataIn, string move)
        {
            var md5 = new MD5CryptoServiceProvider();
            var byt = Encoding.UTF8.GetBytes(move + sDataIn);
            var bytHash = md5.ComputeHash(byt);
            md5.Clear();
            return bytHash.Aggregate("", (c, t) => string.Format("{0}{1}", c, t.ToString("x").PadLeft(2, '0')));
        }

        /// <summary>
        /// 将字符串编码成ASCII数组后进行MD5加密, 然后再编码成ASCII字符串
        /// </summary>
        /// <param name="strEnc"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string strEnc)
        {
            var md5 = new MD5CryptoServiceProvider();
            string result = Encoding.ASCII.GetString(md5.ComputeHash(Encoding.ASCII.GetBytes(strEnc)));
            return result;
        }
    }
}