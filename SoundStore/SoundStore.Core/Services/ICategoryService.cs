using SoundStore.Core.Commons;
using SoundStore.Core.Models.Responses;

namespace SoundStore.Core.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetCategories();

        PaginatedList<CategoryResponse> GetCategories(string? name, int pageNumber, int pageSize);

        Task<CategoryResponse?> GetCategory(int id);

        Task<bool> AddCategory(string name, string description);

        Task<bool> UpdateCategory(int id, string name, string description);

        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="id">Category's id</param>
        /// <returns></returns>
        Task<bool> DeleteCategory(int id);

        Task<bool> AddSubCategory(int categoryId, string name);

        Task<bool> UpdateSubCategory(int id, string name, int? categoryId);

        /// <summary>
        /// Delete subcategory by id
        /// </summary>
        /// <param name="id">Subcategory's id</param>
        /// <returns>State of the operation</returns>
        Task<bool> DeleteSubCategory(int id);
    }
}
