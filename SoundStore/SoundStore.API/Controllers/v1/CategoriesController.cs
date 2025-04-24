using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SoundStore.Core.Commons;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Controllers.v1
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get categories
        /// </summary>
        /// <returns></returns>
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<List<CategoryResponse>>>> GetCategories()
        {
            var response = await _categoryService.GetCategories();
            return Ok(new ApiResponse<List<CategoryResponse>>
            {
                IsSuccess = true,
                Message = "Fetch category successfully!",
                Value = response
            });
        }

        /// <summary>
        /// Get all categories including subcategories
        /// </summary>
        /// <param name="pageNumer">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="name">Category's name</param>
        /// <returns></returns>
        [HttpGet("categories/pageNumber/{pageNumer}/pageSize/{pageSize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public ActionResult<ApiResponse<PaginatedList<CategoryResponse>>> GetCategories(int pageNumer = 1,
            int pageSize = 10, string? name = null)
        {
            var categories = _categoryService.GetCategories(name, pageNumer, pageSize);
            return Ok(new ApiResponse<PaginatedList<CategoryResponse>>
            {
                IsSuccess = true,
                Message = "Fetch data successfully",
                Value = categories
            });
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="id">Category's id</param>
        /// <returns></returns>
        [HttpGet("category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategory(id);

            return Ok(new ApiResponse<CategoryResponse>
            {
                IsSuccess = true,
                Message = "Fetch data successfully",
                Value = category
            });
        }

        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="id">Category's id</param>
        /// <param name="request">Updated request model</param>
        /// <returns></returns>
        [HttpPut("category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCategory(int id, CategoryUpdatedRequest request)
        {
            var result = await _categoryService.UpdateCategory(id, request.Name);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Update category successfully"
            });
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="request">Category's name</param>
        /// <returns></returns>
        [HttpPost("category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> CreateCategory(CategoryCreatedRequest request)
        {
            var result = await _categoryService.AddCategory(request.Name);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Create category successfully"
            });
        }

        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="id">Category's id</param>
        /// <returns></returns>
        [HttpDelete("category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Delete category successfully"
            });
        }

        [HttpPost("category/{id}/sub-category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> CreateSubCategory(int id, SubCategoryCreatedRequest request)
        {
            var result = await _categoryService.AddSubCategory(id, request.Name);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Create sub category successfully"
            });
        }

        [HttpDelete("sub-category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteSubCategory(int id)
        {
            var result = await _categoryService.DeleteSubCategory(id);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Delete sub category successfully"
            });
        }

        [HttpPut("sub-category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateSubCategory(int id, SubCategoryUpdatedRequest request)
        {
            var result = await _categoryService.UpdateSubCategory(id, request.Name, request.CategoryId);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Update sub category successfully"
            });
        }
    }
}
