using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Karpinski_XY.Features.Identity
{
    public class IdentityController : ApiController
    {
        private readonly UserManager<User> userManager;
        private readonly AppSettings appSettings;
        private readonly IIdentityService identityService;

        public IdentityController(UserManager<User> userManager,
            IOptions<AppSettings> appSettings,
            IIdentityService identityService)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
            this.identityService = identityService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost]
        [Route(nameof(Login))]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login(LoginRequestModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return Unauthorized();
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                return Unauthorized();
            }

            var token = this.identityService.GenerateJWTToken(user, this.appSettings.Secret);
            return new { Token = token };
        }
    }
}
