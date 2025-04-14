namespace SoundStore.Core.Commons
{
    public class PaginatedList<T>
    {
        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public int TotalItems { get; private set; }

        public int TotalPages { get; private set; }

        public List<T> Items { get; private set; } = [];

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
    }
}
