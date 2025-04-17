using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SoundStore.Service
{
    public class ProductRatingService(IUnitOfWork unitOfWork,
        ILogger<ProductRatingService> logger,
        UserClaimsService userClaimsService) : IProductRatingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ProductRatingService> _logger = logger;
        private readonly UserClaimsService _userClaimsService = userClaimsService;

        public async Task<bool> AddRating(ProductRatingRequest request)
        {
            try
            {
                var currentUserId = _userClaimsService.GetClaim(JwtRegisteredClaimNames.Sid);
                if (string.IsNullOrEmpty(currentUserId))
                    throw new UnauthorizedAccessException("Invalid user's token!");

                var productRepository = _unitOfWork.GetRepository<Product>();
                var product = await productRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId)
                        ?? throw new KeyNotFoundException("Product not found!");

                product.Ratings.Add(new Rating
                {
                    ProductId = request.ProductId,
                    Comment = request.Comment,
                    RatingPoint = request.Point,
                    UserId = currentUserId,
                });
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<ProductRatingResponse> GetRatingOfAProduct(long productId)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetAll()
                .AsNoTracking()
                .Include(p => p.Ratings)
                .Where(p => p.Id == productId)
                .Select(p => new ProductRatingResponse
                {
                    ProductId = p.Id,
                    RatingPoint = (decimal)p.Ratings.Average(r => r.RatingPoint),
                    Comment = p.Ratings.Select(r => r.Comment).ToList()
                }).FirstOrDefaultAsync() 
                    ?? throw new KeyNotFoundException("No rating data for this product!");

            return product;
        }
    }
}
