using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.Identity;
using Microsoft.AspNetCore.Identity;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IIdentityService
    {
        Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model);
        Task<Result<IdentityResult>> RegisterAsync(RegisterRequestModel model);
    }
}
