using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY_Server.Features.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY.Features.Identity
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
            var result = await this.identityService.RegisterAsync(model);

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
