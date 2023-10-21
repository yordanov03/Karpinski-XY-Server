using Microsoft.AspNetCore.Identity;

namespace Karpinski_XY_Server.Features.Identity.Models
{
    public class LoginResponseModel
    {
        public IdentityResult IdentityResult { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Id { get; set; }
    }

}
