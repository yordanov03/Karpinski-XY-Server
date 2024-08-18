using Karpinski_XY.Infrastructure.Extensions;
using Karpinski_XY_Server.Services.Contracts;
using System.Security.Claims;

namespace Karpinski_XY_Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => user = httpContextAccessor.HttpContext?.User;

        public string GetUserId()
        => user
            ?.Identity
            ?.Name;

        public string GetUserName()
        => user
            ?.GetId();
    }
}
