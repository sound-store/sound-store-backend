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
    public class UsersController(IUserService userService) : BaseApiController
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Get customers with pagination (Admin)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("customers/pageNumber/{pageNumber}/pageSize/{pageSize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<PaginatedList<CustomerInfoResponse>>>> GetCustomers(int pageNumber,
            int pageSize,
            string? name = null)
        {
            var customers = await _userService.GetCustomers(name, pageNumber, pageSize);
            return Ok(new ApiResponse<PaginatedList<CustomerInfoResponse>>
            {
                IsSuccess = true,
                Message = "Fetch customer successfully",
                Value = customers
            });
        }

        /// <summary>
        /// Get customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("customers/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<CustomerDetailedInfoResponse>> GetCustomer(string id)
        {
            var customer = await _userService.GetCustomer(id);
            return Ok(new ApiResponse<CustomerDetailedInfoResponse>
            {
                IsSuccess = true,
                Message = "Fetch customer successfully!",
                Value = customer
            });
        }

        /// <summary>
        /// Register user (Admin)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("admin/user-registration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> AddUser([FromBody] AddedUserRequest value)
        {
            var result = await _userService.AddUser(value);
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Add successfully!"
            });
        }

        /// <summary>
        /// Update customer's status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPatch("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> UpdateCustomerState(string id,
            [FromQuery] string status)
        {
            var result = await _userService.UpdateStatus(id, status);
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Update successfully!"
            });
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCustomer(string id)
        {
            var result = await _userService.DeleteUser(id);
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "Delete customer successfully"
            });
        }
    }
}
