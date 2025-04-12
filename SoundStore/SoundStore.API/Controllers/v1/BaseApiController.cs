using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SoundStore.API.Controllers.v1
{
    [Route("api/sound-store")]
    [ApiController]
    [ApiVersion(1)]
    public class BaseApiController : ControllerBase
    {
    }
}
