using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Common.Helper
{
    public static class DictionaryHelper
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            if (null == dic)
            {
                throw new ArgumentException("dic");
            }
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            throw new ArgumentOutOfRangeException("key", key, "关键字不存在于给定的字典中");
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue)
        {
            if (null == dic)
            {
                throw new ArgumentException("dic");
            }
            TValue result = defaultValue;
            if (null == result)
            {
                result = default(TValue);
            }
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return result;
        }
    }
}
