using Microsoft.AspNetCore.Mvc;
using SalonManagement.Core.Common;

namespace SalonManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }

            return BadRequest(new { error = result.Error });
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { error = result.Error });
        }
    }
}