using Karpinski_XY_Server.Dtos.Identity;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost]
        [Route("register", Name = "regsiter")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            var result = await identityService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("login", Name = "login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseModel))]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var result = await identityService.LoginAsync(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }
            return Unauthorized(result.Errors);
        }

    }
}
