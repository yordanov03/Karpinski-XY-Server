using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IinquiryEmailSenderService
    {
        Task<string> SendEmailAsync(ContactDto inquiry);
    }
}
