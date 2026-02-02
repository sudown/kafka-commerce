using Microsoft.AspNetCore.Mvc;
using SistemaPedidos.API.HttpModels;

namespace SistemaPedidos.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ProcessResult<T>(ResultPattern<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Data == null) return NoContent();

                return Ok(result.Data);
            }

            return StatusCode((int)result.ErrorDetails!.Status!, result.ErrorDetails);
        }
    }
}
