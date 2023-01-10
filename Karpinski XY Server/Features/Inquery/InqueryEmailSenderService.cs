using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;

namespace Karpinski_XY_Server.Features.Inquery
{
    public class InqueryEmailSenderService : IInqueryEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;

        public InqueryEmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<string> SendEmailAsync(InqueryDto inquery)
        {
            var caseNumber = new Random().Next(100,99999);

            // send message to Requestor
            var messageToRequestor = new MimeMessage();
            messageToRequestor.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            messageToRequestor.To.Add(MailboxAddress.Parse(inquery.Email));
            messageToRequestor.Subject = $"Inquery: {caseNumber} " + inquery.Subject;
            messageToRequestor.Body = new TextPart("plain")
            {
                Text = EmailTemplates.RequestorConfirmationTemplate(inquery.Name, caseNumber)
            };


            //send message to artist

            var messageToArtist = new MimeMessage();
            messageToArtist.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            messageToArtist.To.Add(MailboxAddress.Parse("svetoslav.yordanov.003@gmail.com"));
            messageToArtist.Subject = $"Inquery: {caseNumber} " + inquery.Subject;
            messageToArtist.Body = new TextPart("plain")
            {
                Text = EmailTemplates.NotificationToArtist(inquery, caseNumber)
            };


            var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                await client.AuthenticateAsync(new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password));
                await client.SendAsync(messageToRequestor);
                await client.SendAsync(messageToArtist);
                await client.DisconnectAsync(true);
                return "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
