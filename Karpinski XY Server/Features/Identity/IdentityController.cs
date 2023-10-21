using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        [Route("register", Name ="regsiter")]
        [AllowAnonymous]
        public async Task<IdentityResult> Register(RegisterRequestModel model)
        {
            return await this.identityService.RegisterAsync(model);
        }

        [HttpPost]
        [Route("login", Name ="login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var result = await identityService.LoginAsync(model);
            if (result.IdentityResult.Succeeded)
            {
                return Ok(result);
            }
            return Unauthorized(result.IdentityResult.Errors);
        }
    }
}
