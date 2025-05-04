using SoundStore.Core.Commons;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;

namespace SoundStore.Core.Services
{
    public interface IProductService
    {
        PaginatedList<ProductResponse> GetProducts(
            int pageNumber,
            int pageSize,
            ProductFilterParameters parameters,
            string sortByPrice
        );

        PaginatedList<ProductResponse> GetProductByCategory(int categoryId,
            int pageSize,
            int pageNumber);

        PaginatedList<ProductResponse> GetProductBySubCategory(int subCategoryId,
            int pageNumber,
            int pageSize);

        Task<ProductResponse?> GetProduct(long id);

        Task<bool> AddProduct(ProductCreatedRequest request);

        Task<bool> UpdateProduct(long id, ProductUpdatedRequest request);

        Task<bool> DeleteProduct(long productId);
    }
}
