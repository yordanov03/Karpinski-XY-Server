using Microsoft.Extensions.Options;
using System.Net;
using MimeKit;
using Karpinski_XY_Server.Features.Inquiry.Models;
using System.Net.Mail;

namespace Karpinski_XY_Server.Features.inquiry
{
    public class InquiryEmailSenderService : IInquiryEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<InquiryEmailSenderService> _logger;
        private static readonly Random _random = new Random();

        public InquiryEmailSenderService(IOptions<SmtpSettings> smtpSettings, 
            ILogger<InquiryEmailSenderService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task<string> SendEmailAsync(InquiryDto inquiry)
        {
            var caseNumber = _random.Next(100, 99999);

            var messageToRequestor = new MimeMessage();
            messageToRequestor.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            messageToRequestor.To.Add(MailboxAddress.Parse(inquiry.Email));
            messageToRequestor.Cc.Add(MailboxAddress.Parse(_smtpSettings.CCEmail));
            messageToRequestor.Subject = $"Inquiry: {caseNumber} " + inquiry.Subject;
            messageToRequestor.Body = new TextPart("html") { Text = EmailTemplates.RequestorConfirmationTemplate(inquiry) };

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                    await client.AuthenticateAsync(new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password));
                    await client.SendAsync(messageToRequestor);

                    _logger.LogInformation("Email sent successfully to {Email}", inquiry.Email);

                    await client.DisconnectAsync(true);
                }

                return "Email Sent Successfully";
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "An SMTP error occurred while sending email to {Email}", inquiry.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error occurred while sending email to {Email}", inquiry.Email);
                throw;
            }
        }
    }
}
