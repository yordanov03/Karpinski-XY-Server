using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
    }
}
