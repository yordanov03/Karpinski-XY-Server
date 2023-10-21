using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY_Server.Features.Identity.Models;
using Karpinski_XY_Server.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace Karpinski_XY.Features.Identity
{
    public interface IIdentityService
    {
        Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model);
        Task<Result<IdentityResult>> RegisterAsync(RegisterRequestModel model);
    }
}
