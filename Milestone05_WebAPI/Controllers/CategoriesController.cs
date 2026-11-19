using Microsoft.AspNetCore.Mvc;
using Milestone05_WebAPI.Models;

namespace Milestone05_WebAPI.Controllers;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public ActionResult<List<string>> GetAll()
    {
        return Ok(Enum.GetNames<Category>().ToList());
    }
}
