using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY_Server.Features.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Karpinski_XY.Features.Identity
{
    public interface IIdentityService
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel model);
        Task<IdentityResult> RegisterAsync(RegisterRequestModel model);
    }
}
