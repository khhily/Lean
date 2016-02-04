using System.Linq;

namespace WGX.Common.Helper
{
    public static class QueryableHelper
    {
        /// <summary>
        /// 分页,分页之前要排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static IQueryable<T> DoPage<T>(this IQueryable<T> source, Pager pager)
        {
            pager.Count = source.Count();

            return source
                .Skip((pager.Page ?? 0) * pager.PageSize)
                .Take(pager.PageSize).AsQueryable();
        }
    }
}
