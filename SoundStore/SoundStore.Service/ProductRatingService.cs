using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Services;

namespace SoundStore.Service
{
    public class ProductRatingService(IUnitOfWork unitOfWork,
        ILogger<ProductRatingService> logger) : IProductRatingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ProductRatingService> _logger = logger;


    }
}
