using System;
using System.Collections.Generic;
using System.Linq;

namespace WGX.Common
{
    public class Pagination
    {
        private int _total;
        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value < 0 ? 0 : value;
            }
        }

        private int _currPage = 1;
        /// <summary>
        /// 从１开始
        /// </summary>
        public int CurrPage
        {
            get
            {
                return _currPage;
            }
            set
            {
                _currPage = value < 1 ? 1 : value;
            }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value < 1 ? 1 : value;
            }
        }

        private int _labelCount = 10;
        public int LableCount
        {
            get
            {
                return _labelCount;
            }
            set
            {
                _labelCount = value < 1 ? 1 : value;
            }
        }


        public int TotalPage
        {
            get
            {
                return (int)Math.Ceiling(((decimal)Total / PageSize));
            }
        }

        private int[] _pageSizes = { 10, 50, 100, 200, 500 };

        public int[] PageSizes
        {
            get
            {
                return _pageSizes;
            }
            set
            {
                _pageSizes = value.Where(i => i > 0).ToArray();
            }
        }

        public List<PageItem> GetPageItems()
        {
            int begin = CurrPage - LableCount / 2;
            if (begin < 1)
                begin = 1;
            int end = begin + _labelCount;
            if (end > TotalPage)
            {
                end = TotalPage + 1;
                begin = end - LableCount;
                if (begin < 1)
                    begin = 1;
            }
            if (end - begin > 0)
            {
                var items = Enumerable.Range(begin, end - begin)
                    .Select(i => new PageItem
                    {
                        Page = i,
// ReSharper disable once SpecifyACultureInStringConversionExplicitly
                        Label = i.ToString(),
                        IsCurrPage = i == CurrPage
                    }).ToList();

                if (CurrPage > 1)
                {
                    items.Insert(0, new PageItem
                    {
                        Label = "<",
                        Page = CurrPage - 1
                    });
                    items.Insert(0, new PageItem
                    {
                        Label = "|<",
                        Page = 1
                    });
                }

                if (CurrPage < TotalPage)
                {

                    items.Add(new PageItem
                    {
                        Label = ">",
                        Page = CurrPage + 1
                    });

                    items.Add(new PageItem
                    {
                        Label = ">|",
                        Page = TotalPage
                    });
                }
                return items;
            }
            return new List<PageItem>();
        }
    }
}
