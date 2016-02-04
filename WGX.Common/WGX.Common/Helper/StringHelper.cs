using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;

namespace WGX.Common.Helper
{
    public static class StringHelper
    {
        #region To int
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue)
        {
            int v;
            return int.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return str.ToInt(0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToIntOrNull(this string str, int? defaultValue)
        {
            int v;
            return int.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? ToIntOrNull(this string str)
        {
            return str.ToIntOrNull(null);
        }

        /// <summary>
        /// 转换为Int ，取字符串中的第一个数为准
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int SmartToInt(this string str, int defaultValue)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            //Match ma = Regex.Match(str, @"(\d+)");
            var ma = Regex.Match(str, @"((-\s*)?\d+)");
            return ma.Success ? ma.Groups[1].Value.Replace(" ", "").ToInt(defaultValue) : defaultValue;
        }
        #endregion

        #region To Float
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToFloat(this string str, float defaultValue)
        {
            float v;
            return float.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            return str.ToFloat(0f);
        }

        /// <summary>
        /// 智能转换为float ，取字符串中的第一个数
        /// 可转换 a1.2, 0.12 1.2aa , 1.2e+7
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float SmartToFloat(this string str, float defaultValue)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            //Regex rx = new Regex(@"((\d+)(\.?((?=\d)\d+))?(e\+\d)*)");
            var rx = new Regex(@"((-\s*)?(\d+)(\.?((?=\d)\d+))?(e[\+-]?\d+)*)");
            var ma = rx.Match(str);
            return ma.Success ? ma.Groups[1].Value.Replace(" ", "").ToFloat(defaultValue) : defaultValue;
        }
        #endregion

        #region To Decimal

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal defaultValue)
        {
            decimal v;
            return decimal.TryParse(str, NumberStyles.Any, null, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str)
        {
            return str.ToDecimal(0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal? ToDecimalOrNull(this string str)
        {
            decimal temp;
            return decimal.TryParse(str, out temp) ? (decimal?) temp : null;
        }

        /// <summary>
        /// 智能转换为 float ，取字串中的第一个数位串
        /// 可转换 a1.2, 0.12 1.2aa , 1.2e+7
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal SmartToDecimal(this string str, decimal defaultValue)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            //Regex rx = new Regex(@"((\d+)(\.?((?=\d)\d+))?(e\+\d)*)");
            //Regex rx = new Regex(@"((-\s*)?(\d+)(\.?((?=\d)\d+))?(e[\+-]?\d+)*)");
            var rx = new Regex(@"((-\s*)?(\d+(,\d+)*)(\.?((?=\d)\d+))?(e[\+-]?\d+)*)");
            var ma = rx.Match(str);
            return ma.Success ? ma.Groups[1].Value.Replace(" ", "").Replace(",", "").ToDecimal(defaultValue) : defaultValue;
        }

        #endregion

        #region To byte
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ToByte(this string str, byte defaultValue)
        {
            byte v;
            return byte.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToByte(this string str)
        {
            return str.ToByte(0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte? ToByteOrNull(this string str, byte? defaultValue)
        {
            byte v;
            return byte.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte? ToByteOrNull(this string str)
        {
            return str.ToByteOrNull(null);
        }
        #endregion

        #region To long
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this string str, long defaultValue)
        {
            long v;
            return long.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            return str.ToLong(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long? ToLongOrNull(this string str)
        {
            long temp;
            return long.TryParse(str, out temp) ? (long?) temp : null;
        }

        #endregion

        #region To ulong
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ulong ToUlong(this string str, ulong defaultValue)
        {
            ulong v;
            return ulong.TryParse(str, out v) ? v : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ulong ToUlong(this string str)
        {
            return str.ToUlong(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ulong? ToUlongOrNull(this string str)
        {
            ulong temp;
            return ulong.TryParse(str, out temp) ? (ulong?) temp : null;
        }

        #endregion

        #region ToBool

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string str, bool defaultValue)
        {
            bool b;
            return bool.TryParse(str, out b) ? b : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToBool(this string str)
        {
            return str.ToBool(false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool? ToBoolOrNull(this string str)
        {
            bool temp;
            return bool.TryParse(str, out temp) ? (bool?) temp : null;
        }

        #endregion

        #region ToDouble
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string str, double defaultValue)
        {
            double v;
            return double.TryParse(str, NumberStyles.Any, null, out v) ? v : defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToDouble(this string str)
        {
            return str.ToDouble(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double? ToDoubleOrNull(this string str)
        {
            double temp;
            return double.TryParse(str, out temp) ? (double?) temp : null;
        }

        #endregion

        #region Enum

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public static T ToEnum<T>(this int value, T defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new Exception("T 必须是枚举类型");

            if (Enum.IsDefined(type,  value))
                return (T) Enum.ToObject(type, value);
            if (type.GetCustomAttribute<FlagsAttribute>() == null) return defaultValue;
            T tmp;
            Enum.TryParse(value.ToString(CultureInfo.InvariantCulture), out tmp);
            return !tmp.Equals(default(T)) ? tmp : defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.ToEnum(default(T));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public static T ToEnum<T>(this byte value, T defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new Exception("T 必须是枚举类型");

            if (Enum.IsDefined(type, value))
            {
                return (T)Enum.ToObject(type, value);
            }
            if (type.GetCustomAttribute<FlagsAttribute>() == null) return defaultValue;
            T tmp;
            Enum.TryParse(value.ToString(CultureInfo.InvariantCulture), out tmp);
            return !tmp.Equals(default(T)) ? tmp : defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this byte value) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.ToEnum(default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase) where T : struct, IComparable, IConvertible, IFormattable
        {

            T o;
            var flag = Enum.TryParse(value, ignoreCase, out o);
            if (flag && Enum.IsDefined(typeof(T), o))
                return o;
            if (typeof (T).GetCustomAttribute<FlagsAttribute>() == null) return defaultValue;
            T tmp;
            Enum.TryParse(value.ToString(CultureInfo.InvariantCulture), out tmp);
            return !tmp.Equals(default(T)) ? tmp : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.ToEnum(defaultValue, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.ToEnum(default(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.ToEnum(default(T), ignoreCase);
        }


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this string value, int defaultValue, bool ignoreCase) where T : struct, IComparable, IConvertible, IFormattable
        {
            T o;
            var flag = Enum.TryParse(value, ignoreCase, out o);
            return flag ? Convert.ToInt32(o) : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this string value, int defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.GetEnumValue<T>(defaultValue, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this T value, int defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            return !typeof(T).IsEnum ? defaultValue : Convert.ToInt32(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(this T value) where T : struct, IComparable, IConvertible, IFormattable
        {
            return value.GetEnumValue(0);
        }

        #endregion

        #region IPAddress
        /// <summary>
        /// 如果轉換失敗，返回 IPAddress.None
        /// </summary>
        /// <param name="ipstring"></param>
        /// <returns></returns>
        public static IPAddress ToIPAddress(this string ipstring)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipstring, out ip) ? ip : IPAddress.None;
        }

        #endregion

        #region DateTime

        /// <summary>
        /// 轉換為日期，如果轉換失敗，返回預設值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static DateTime? ToDateTimeOrNull(this string str, DateTime? defaultValue = null)
        {
            if (defaultValue == null) throw new ArgumentNullException("defaultValue");
            DateTime d;
            if (DateTime.TryParse(str, out d))
                return d;
            return DateTime.TryParseExact(str, new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyyMMdd HH:mm:ss", "yyyy/MM/dd", "yyyy'/'MM'/'dd HH:mm:ss", "MM'/'dd'/'yyyy HH:mm:ss", "yyyy-M-d", "yyy-M-d hh:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d) ? d : defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dateFmt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeOrNull(this string str, string dateFmt, DateTime? defaultValue = null)
        {
            DateTime d;
            //if (DateTime.TryParse(str, out d))
            //    return d;
            //else {
            return DateTime.TryParseExact(str, dateFmt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d) ? d : defaultValue;
            //}        
        }

        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// 轉換日期，轉換失敗時，返回 defaultValue
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime defaultValue = default(DateTime))
        {
            DateTime d;
            if (DateTime.TryParse(str, out d))
                return d;
            if (DateTime.TryParseExact(str, new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyyMMdd HH:mm:ss", "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "MM/dd/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d))
                return d;
            return default(DateTime) == defaultValue ? MinDate : defaultValue;
        }

        /// <summary>
        /// 按給定日期格式進行日期轉換
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dateFmt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, string dateFmt, DateTime defaultValue)
        {
            DateTime d;
            //if (DateTime.TryParse(str, out d))
            //    return d;
            //else {
            return DateTime.TryParseExact(str, dateFmt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d) ? d : defaultValue;
            //}            
        }

        /// <summary>
        /// 轉換為日期，轉換失敗時，返回null
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeOrNull(this string str)
        {
            return str.ToDateTimeOrNull(null);
        }

        /// <summary>
        /// 轉換日期，轉換失敗時，返回當前時間
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return str.ToDateTime(DateTime.Now);
        }

        /// <summary>
        /// 是否為日期型字串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str)
        {
            //return Regex.IsMatch(str, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
            DateTime d;
            return DateTime.TryParseExact(str, new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyyMMdd HH:mm:ss", "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "MM/dd/yyyy", "MM/dd/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out d);
        }

        #endregion

        #region ToTimeSpan
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string str, TimeSpan defaultValue)
        {
            TimeSpan t;
            return TimeSpan.TryParse(str, out t) ? t : defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string str)
        {
            return str.ToTimeSpan(new TimeSpan());
        }
        #endregion

        #region Guid
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            Guid g;
            return Guid.TryParse(str, out g) ? g : Guid.Empty;
        }

        #endregion

        #region Url

        /// <summary>
        /// 從 URL 中取出鍵的值, 如果不存在,返回空
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string ParseString(this string s, string key, bool ignoreCase)
        {
            if (s == null)
                return ""; //必須這樣,請不要修改

            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            Dictionary<string, string> kvs = s.ParseString(ignoreCase);
            key = ignoreCase ? key.ToLower() : key;
            if (kvs.ContainsKey(key))
            {
                return kvs[key];
            }
            return "";
        }

        /// <summary>
        /// 從URL中取 Key / Value
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseString(this string s, bool ignoreCase)
        {
            //必須這樣,請不要修改
            if (string.IsNullOrEmpty(s))
            {
                return new Dictionary<string, string>();
            }

            if (s.IndexOf('?') != -1)
            {
                s = s.Remove(0, s.IndexOf('?'));
            }

            var kvs = new Dictionary<string, string>();
            var reg = new Regex(@"[\?&]?(?<key>[^=]+)=(?<value>[^\&]*)", RegexOptions.Compiled | RegexOptions.Multiline);
            var ms = reg.Matches(s);
            foreach (Match ma in ms)
            {
                var key = ignoreCase ? ma.Groups["key"].Value.ToLower() : ma.Groups["key"].Value;
                if (kvs.ContainsKey(key))
                {
                    kvs[key] += "," + ma.Groups["value"].Value;
                }
                else
                {
                    kvs[key] = ma.Groups["value"].Value;
                }
            }

            return kvs;
        }

        /// <summary>
        /// 設置 URL中的 key
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SetUrlKeyValue(this string url, string key, string value, Encoding encode = null)
        {
            if (url == null)
                url = "";
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (value == null)
                value = "";
            if (null == encode)
                encode = Encoding.UTF8;
            //if (!string.IsNullOrEmpty(url.ParseString(key, true).Trim())) {
            if (!url.ParseString(true).ContainsKey(key.ToLower()))
                return url + (url.IndexOf('?') > -1 ? "&" : "?") + key + "=" + HttpUtility.UrlEncode(value, encode);
            var reg = new Regex(@"([\?\&])(" + key + @"=)([^\&]*)(\&?)", RegexOptions.IgnoreCase);
            //　如果 value 前几个字符是数字，有BUG
            //return reg.Replace(url, "$1$2" + HttpUtility.UrlEncode(value, encode) + "$4");

            return reg.Replace(url, ma => ma.Success ? string.Format("{0}{1}{2}{3}", ma.Groups[1].Value, ma.Groups[2].Value, HttpUtility.UrlEncode(value, encode), ma.Groups[4].Value) : "");
        }


        /// <summary>
        /// 修正URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string FixUrl(this string url)
        {
            return url.FixUrl("");
        }

        /// <summary>
        /// 修正URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public static string FixUrl(this string url, string defaultPrefix)
        {
            // 必須這樣,請不要修改
            if (url == null)
                url = "";

            if (defaultPrefix == null)
                defaultPrefix = "";
            var tmp = url.Trim();
            if (!Regex.Match(tmp, "^(http|https):").Success)
            {
                return string.Format("{0}/{1}", defaultPrefix, tmp);
            }
            tmp = Regex.Replace(tmp, @"(?<!(http|https):)[\\/]+", "/").Trim();
            return tmp;
        }
        #endregion

        #region mix
        /// <summary>
        /// 如果 str 是 NullOrWhiteSpace ， 就返回 defaultValue
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Nvl(this string str, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(str) ? defaultValue : str;
        }

        /// <summary>
        /// 獲取用於 Javascript 的安全字串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string JsSafeString(this string str)
        {
            return String.IsNullOrEmpty(str) ? "" : str.ToString(CultureInfo.InvariantCulture).Replace("'", "\\'").Replace("\"", "&quot;");
        }

        /// <summary>
        /// 清除两个以上的空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearupSpace(this string str)
        {
            return Regex.Replace(str.Replace("&nbsp;", " "), @"\s{2,}", " ").Trim();
        }

        /// <summary>
        /// 安全的转换为大写
        /// <remarks>
        /// 如果直接用 ToUpper , 当为 null 的时候,会报 NullReference
        /// </remarks>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeToUpper(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : str.ToUpper();
        }

        /// <summary>
        /// 安全的转换为小写
        /// <remarks>
        /// 如果直接用 ToUpper , 当为 null 的时候,会报 NullReference
        /// </remarks>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeToLower(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : str.ToLower();
        }

        #endregion

        #region Json 与 C# 对象
        public static List<T> JsonToList<T>(string jsonStr)
        {
            var serializer = new JavaScriptSerializer();
            var objs = serializer.Deserialize<List<T>>(jsonStr);
            return objs;
        }


        public static T JsonToObj<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// List对象转化成Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> list)
        {
            var json = new StringBuilder();
            json.Append("[");
            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var obj = Activator.CreateInstance<T>();
                    var pi = obj.GetType().GetProperties();
                    json.Append("{");
                    for (var j = 0; j < pi.Length; j++)
                    {
                        var value = "";
                        if (pi[j] != null && pi[j].GetValue(list[i], null) != null)
                        {
                            var type = pi[j].GetValue(list[i], null).GetType();
                            value = StringFormat(pi[j].GetValue(list[i], null).ToString(), type);
                        }
                        else
                        {
                            value = "\"" + value + "\"";
                        }

                        var propertyInfo = pi[j];
                        if (propertyInfo != null) json.Append("\"" + propertyInfo.Name + "\":" + value);
                        if (j < pi.Length - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < list.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]");
            return json.ToString();
        }
        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }
            else if (type != typeof(string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>json字符串</returns>
        private static string String2Json(String s)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
