using System.Collections;
using System.Collections.Generic;

namespace DatingApp.API.Helps
{
    public class PageList<T>: List<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }

        public PageList(IEnumerable<T> items, int currentPage, int pageSize, int totalPage)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPage = totalPage;
            this.AddRange(items);
        }
    }
}