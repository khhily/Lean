
namespace WGX.Common
{
    public class Pager
    {
        private int _pageSize = 20;

        /// <summary>
        /// 每页数据大小,默认20
        /// </summary>
        [DisableBinding]
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value <= 0 ? 20 : value;
            }
        }


        private int _page;
        /// <summary>
        /// 第几页
        /// </summary>
        public int? Page
        {
            get
            {
                return _page;
            }
            set
            {
                //this.page = value < 0 ? 0 : value;
                _page = value == null ? 0 : (value.Value < 0 ? 0 : value.Value);
            }
        }

        /// <summary>
        /// 查询结果条数
        /// </summary>
        [DisableBinding]
        public int Count
        {
            get;
            set;
        }
    }
}
