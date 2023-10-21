using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY.Features
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
    }
}
