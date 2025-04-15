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
    public class AuthController(IUserService userService) : BaseApiController
    {
        private readonly IUserService _userService = userService;

        // GET: api/<AuthController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<AuthController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("users/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
            [FromBody] Microsoft.AspNetCore.Identity.Data.LoginRequest value
        )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _userService.Login(value.Email, value.Password);
            return Ok(new ApiResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "Success",
                Value = response
            });
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("users/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] UserRegistration value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.RegisterUser(value);
            if (!result) return BadRequest();
            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "User registered successfully!",
            });
        }
        
        [HttpGet("users/me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion(1)]
        public async Task<ActionResult<LoginResponse>> GetProfile()
        {
            var response = await _userService.GetProfile();
            return Ok(new ApiResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "Success",
                Value = response
            });
        }

        // PUT api/<AuthController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<AuthController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
