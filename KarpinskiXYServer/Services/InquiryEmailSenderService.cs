﻿using Microsoft.Extensions.Options;
using System.Net;
using MimeKit;
using Karpinski_XY_Server.Services.Contracts;
using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Helpers;

namespace Karpinski_XY_Server.Services
{
    public class InquiryEmailSenderService : IinquiryEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;

        public InquiryEmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<string> SendEmailAsync(ContactDto inquiry)
        {
            var caseNumber = new Random().Next(100, 99999);

            // send message to Requestor
            var messageToRequestor = new MimeMessage();
            messageToRequestor.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            messageToRequestor.To.Add(MailboxAddress.Parse(inquiry.Email));
            messageToRequestor.Cc.Add(MailboxAddress.Parse(_smtpSettings.CCEmail));
            messageToRequestor.Subject = $"inquiry: {caseNumber} " + inquiry.Subject;
            messageToRequestor.Body = new TextPart("html")
            {
                Text = EmailTemplates.RequestorConfirmationTemplate(inquiry),
            };

            var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                await client.AuthenticateAsync(new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password));
                await client.SendAsync(messageToRequestor);
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
