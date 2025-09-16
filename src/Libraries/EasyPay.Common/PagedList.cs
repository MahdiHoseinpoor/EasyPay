using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common
{
    public class PagedList<T> : IPagedList<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages => PageSize != 0 ? (int)Math.Ceiling(TotalCount * 1.0 / PageSize) : 0;
        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => PageIndex + 1 < TotalPages;
        public List<T> Items { get; } = new List<T>();
        public PagedList() { }
        public PagedList(List<T> source, int pageIndex, int pageSize, int totalCount)
        {
            Items = source ?? new List<T>();
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;

        }
    }
}
