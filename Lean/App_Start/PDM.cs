using WGX.Common;

// ReSharper disable once CheckNamespace
namespace Lean
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PDM<T> where T : class
    {
        public T Data
        {
            get;
            set;
        }
    }

    public class PDM<TData, TSearch> : PDM<TData>
        where TData : class
        where TSearch : BaseQuery
    {
        public TSearch Condition
        {
            get;
            set;
        }
    }

    public static class PDM
    {
        public static PDM<TData, TCondition> Create<TData, TCondition>(TData data, TCondition search)
            where TData : class
            where TCondition : BaseQuery
        {
            return new PDM<TData, TCondition>
            {
                Data = data,
                Condition = search
            };
        }
    }
}
