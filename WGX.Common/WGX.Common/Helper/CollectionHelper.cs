using System;
using System.Collections.Generic;
using System.Linq;

namespace WGX.Common.Helper
{
    public static class CollectionHelper
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dic , TKey key, TValue defaultValue)
        {
            if (null == dic) throw new ArgumentException("dic");
            return dic.Keys.Contains(key) ? dic[key] : defaultValue;
        }

        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if(null == dic) throw new ArgumentException("dic");
            if (dic.Keys.Contains(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static IEnumerable<T> DoPage<T>(this IEnumerable<T> source, Pager pager)
        {
            var enumerable = source as IList<T> ?? source.ToList();
            pager.Count = enumerable.Count();

            return enumerable
                .Skip((pager.Page ?? 0) * pager.PageSize)
                .Take(pager.PageSize).AsQueryable();
        }
    }
}
