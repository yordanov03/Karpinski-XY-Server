namespace Karpinski_XY_Server.Features.Inquery
{
    public interface IInqueryEmailSenderService
    {
        Task<string> SendEmailAsync(InqueryDto inquery);
    }
}
