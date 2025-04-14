using SoundStore.Core.Commons;

namespace SoundStore.Infrastructure.Helpers
{
    /// <summary>
    /// Helper class for creating paginated lists.
    /// </summary>
    public class PaginationHelper
    {
        public static PaginatedList<T> CreatePaginatedList<T>(IQueryable<T> source, 
            int pageIndex, 
            int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
