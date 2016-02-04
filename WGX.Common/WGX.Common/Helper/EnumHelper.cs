using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace WGX.Common.Helper
{
    public static class EnumHelper
    {
        private static IEnumDescriptionProvider Provider = null;

        public static void Init(IEnumDescriptionProvider provider)
        {
            Provider = provider;
        }
        ///// <summary>
        ///// 获取枚举值的Description特性
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static String GetDescription(this Enum value)
        //{
        //    var enumType = value.GetType();
        //    var field = enumType.GetField(value.ToString());
        //    if (field == null) return "";
        //    var attrs = field.GetCustomAttributes(typeof (DescriptionAttribute));
        //    if (attrs != null)
        //    {
        //        foreach (DescriptionAttribute attr in attrs.OfType<DescriptionAttribute>())
        //            return (attr).Description;
        //    }
        //    return field.Name;
        //}

        public static string GetDescription(this Enum e)
        {
            var desc = "";
            if (Provider != null)
            {
                var key = string.Format("{0}_{1}_Description", e.GetType().FullName.Replace("+", "_").Replace(".", ""), e.ToString());
                desc = Provider.GetDescription(key);
            }

            if (!string.IsNullOrWhiteSpace(desc))
                return desc;


            FieldInfo fi = e.GetType().GetField(e.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return e.ToString();
        }

        public static string GetDescription<T>(T value) where T : struct, IComparable, IConvertible, IFormattable
        {
            var dic = GetDescriptions<T>();
            return dic.Get(value, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 获取某枚举的全部集合
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetEnumDescriptionsList(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("不是一个有效的枚举类型");
            var ms = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            return ms.Select(m =>
            {
                var desc = ((DescriptionAttribute[])m.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();
                return new SelectListItem
                {
                    Value = m.Name,
                    Text = desc != null ? desc.Description : m.Name
                };
            }).ToList();
        }

        public static Dictionary<string, string> GetDescriptions(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("不是一个有效的枚举类型");
            var ms = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            return ms.Select(m =>
            {
                var desc = ((DescriptionAttribute[])m.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();
                return new
                {
                    K = m.Name,
                    V = desc != null ? desc.Description : m.Name
                };
            }).ToDictionary(a => a.K, a => a.V);
        }

        public static Dictionary<T, string> GetDescriptions<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new ArgumentException("不是一个有效的枚举类型");
            var ms = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            return ms.Select(m =>
            {
                var desc = ((DescriptionAttribute[])m.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();
                return new
                {
                    K = m.Name.ToEnum<T>(),
                    V = desc != null ? desc.Description : m.Name
                };
            }).ToDictionary(a => a.K, a => a.V);
        }
    }
}
