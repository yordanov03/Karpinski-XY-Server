using Karpinski_XY_Server.Features.Inquiry.Models;

namespace Karpinski_XY_Server.Features.inquiry
{
    public interface IInquiryEmailSenderService
    {
        Task<string> SendEmailAsync(InquiryDto inquiry);
    }
}
