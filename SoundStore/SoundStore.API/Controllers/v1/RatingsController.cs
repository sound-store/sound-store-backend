using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundStore.Core.Commons;
using SoundStore.Core.Constants;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Controllers.v1
{
    public class RatingsController : BaseApiController
    {
        private readonly IProductRatingService _productRatingService;

        public RatingsController(IProductRatingService productRatingService)
        {
            _productRatingService = productRatingService;
        }

        /// <summary>
        /// Get rating of a product
        /// </summary>
        /// <param name="id">Product's id</param>
        /// <returns></returns>
        [HttpGet("products/{id}/ratings")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductRatingResponse>>> GetRatingOfAProduct(long id)
        {
            var response = await _productRatingService.GetRatingOfAProduct(id);
            return Ok(new ApiResponse<ProductRatingResponse>
            {
                IsSuccess = true,
                Message = "Fetch product's rating successfully",
                Value = response
            });
        }

        /// <summary>
        /// Add rating of a proudct
        /// </summary>
        /// <param name="request">Model to create a rating</param>
        /// <returns></returns>
        [HttpPost("product/rating")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<string>> AddRating(ProductRatingRequest request)
        {
            var result = await _productRatingService.AddRating(request);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Add rating successfully!"
            });
        }
    }
}
