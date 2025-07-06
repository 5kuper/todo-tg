namespace Utilities.Common
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }

        public required IList<T> Items { get; set; }

        public int NumPages { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
    }
}
