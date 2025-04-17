using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;

namespace SoundStore.Core.Services
{
    public interface IProductRatingService
    {
        Task<ProductRatingResponse> GetRatingOfAProduct(long productId);

        Task<bool> AddRating(ProductRatingRequest request);
    }
}
