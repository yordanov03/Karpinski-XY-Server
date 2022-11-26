using Karpinski_XY.Infrastructure.Extensions;
using System.Security.Claims;

namespace Karpinski_XY.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => this.user = httpContextAccessor.HttpContext?.User;


        public string GetUserId()
        => this.user
            ?.Identity
            ?.Name;

        public string GetUserName()
        => this.user
            ?.GetId();
    }
}
