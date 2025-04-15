using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SoundStore.Core.Commons;
using System.Net;

namespace SoundStore.API.Controllers.v1
{
    [Route("api/sound-store")]
    [ApiController]
    [ApiVersion(1)]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Handle validation errors, replace using ModelState at other controllers
        /// </summary>
        /// <returns></returns>
        protected IActionResult? HandleValidationErrors()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => new ErrorResponse
                    {
                        Message = "Validation failed.",
                        Details = x.ErrorMessage
                    }).FirstOrDefault(); // Or collect all errors if needed

                return new JsonResult(errors) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            return null; // Model is valid
        }

    }
}
