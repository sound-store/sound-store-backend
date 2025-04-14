using SoundStore.Core.Commons;
using SoundStore.Core.Models.Filters;
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

        Task<bool> AddProduct();

        Task<bool> UpdateProduct();

        Task<bool> DeleteProduct();
    }
}
