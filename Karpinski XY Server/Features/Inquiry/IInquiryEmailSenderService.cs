namespace Karpinski_XY_Server.Features.inquiry
{
    public interface IinquiryEmailSenderService
    {
        Task<string> SendEmailAsync(inquiryDto inquiry);
    }
}
