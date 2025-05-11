using Microsoft.AspNetCore.Mvc;

namespace SoundStore.API.Controllers.v1
{
    public class TestableApiController : BaseApiController
    {
        public IActionResult? InvokeValidationHandler()
        {
            return HandleValidationErrors();
        }

        public void AddModelError(string key, string errorMessage)
        {
            ModelState.AddModelError(key, errorMessage);
        }
    }
}
