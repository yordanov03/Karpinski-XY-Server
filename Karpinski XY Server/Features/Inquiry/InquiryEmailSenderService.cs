﻿using Microsoft.Extensions.Options;
using System.Net;
using MimeKit;
using Karpinski_XY_Server.Features.Inquiry.Models;
using System.Net.Mail;
using Karpinski_XY_Server.Infrastructure.Services;
using FluentValidation;

namespace Karpinski_XY_Server.Features.inquiry
{
    public class InquiryEmailSenderService : IInquiryEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IValidator<InquiryDto> _inquiryValidator;
        private readonly ILogger<InquiryEmailSenderService> _logger;
        private static readonly Random _random = new Random();

        public InquiryEmailSenderService(IOptions<SmtpSettings> smtpSettings,
            IValidator<InquiryDto> inquiryValidator,
            ILogger<InquiryEmailSenderService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _inquiryValidator = inquiryValidator;
            _logger = logger;
        }

        public async Task<Result<string>> SendEmailAsync(InquiryDto inquiry)
        {
            var validationResult = _inquiryValidator.Validate(inquiry);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for inquiry. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<string>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var caseNumber = _random.Next(100, 99999);
            var emailResult = new Result<string>();

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

                emailResult = Result<string>.Success("Email Sent Successfully");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "An SMTP error occurred while sending email to {Email}", inquiry.Email);
                emailResult = Result<string>.Fail($"SMTP Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error occurred while sending email to {Email}", inquiry.Email);
                emailResult = Result<string>.Fail($"Unknown Error: {ex.Message}");
            }

            return emailResult;
        }

    }
}
