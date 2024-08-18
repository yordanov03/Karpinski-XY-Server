using Karpinski_XY_Server.Data.Models.Base;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface ISubscriptionService
    {
        Task<Result<string>> AddSubscriberAsync(string email);
    }
}
