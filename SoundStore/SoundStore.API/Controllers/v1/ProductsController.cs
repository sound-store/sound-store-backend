using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Controllers.v1
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="parameters"></param>
        /// <param name="sortByPrice"></param>
        /// <returns></returns>
        [HttpGet("products/pageNumber/{pageNumber}/pageSize/{pageSize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public ActionResult<PaginatedList<ProductResponse>> GetProducts(
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery] ProductFilterParameters? parameters = null,
            [FromQuery] string? sortByPrice = null
        )
        {
            var result = _productService.GetProducts(pageNumber, pageSize, parameters!, sortByPrice!);
            return Ok(new ApiResponse<PaginatedList<ProductResponse>>
            {
                IsSuccess = true,
                Message = "Fetch products successfully",
                Value = result
            });
        }

        /// <summary>
        /// Get products by category with pagination
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("products/category/{id}/pageNumber/{pageNumber}/pageSize/{pageSize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public ActionResult<PaginatedList<ProductResponse>> GetProductByCategory(
            int id,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var result = _productService.GetProductByCategory(id, pageSize, pageNumber);
            return Ok(new ApiResponse<PaginatedList<ProductResponse>>
            {
                IsSuccess = true,
                Message = "Fetch products successfully",
                Value = result
            });
        }

        /// <summary>
        /// Get products by subcategory with pagination
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("products/sub-category/{id}/pageNumber/{pageNumber}/pageSize/{pageSize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public ActionResult<PaginatedList<ProductResponse>> GetProductBySubCategory(
            int id,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var result = _productService.GetProductBySubCategory(id, pageSize, pageNumber);
            return Ok(new ApiResponse<PaginatedList<ProductResponse>>
            {
                IsSuccess = true,
                Message = "Fetch products successfully",
                Value = result
            });
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id">Product's id</param>
        /// <returns></returns>
        [HttpGet("products/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProduct(long id)
        {
            var product = await _productService.GetProduct(id);
            return new ApiResponse<ProductResponse>
            {
                IsSuccess = true,
                Message = "Fetch product successfully",
                Value = product
            };
        }

        /// <summary>
        /// Update the product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("product/{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<string>> UpdateProduct(long id, Product product)
        {
            var result = await _productService.UpdateProduct();
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Update product successfully",
            });
        }

        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="request">Product's data</param>
        /// <returns></returns>
        [HttpPost("product")]
        //[Authorize(Roles = "")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> AddProduct([FromForm] ProductCreatedRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _productService.AddProduct(request);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Add product successfully",
            });
        }

        /// <summary>
        /// Delete existing product
        /// </summary>
        /// <param name="id">Product's id</param>
        /// <returns></returns>
        [HttpDelete("product/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<string>> DeleteProduct(long id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Delete product successfully",
            });
        }
    }
}
