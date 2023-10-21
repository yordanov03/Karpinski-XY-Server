using Karpinski_XY_Server.Features.Inquiry.Models;
using Karpinski_XY_Server.Infrastructure.Services;

namespace Karpinski_XY_Server.Features.inquiry
{
    public interface IInquiryEmailSenderService
    {
        Task<Result<string>> SendEmailAsync(InquiryDto inquiry);
    }
}
