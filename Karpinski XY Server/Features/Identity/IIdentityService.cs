using Karpinski_XY.Models;

namespace Karpinski_XY.Features.Identity
{
    public interface IIdentityService
    {
        string GenerateJWTToken(User user, string secret);
    }
}
