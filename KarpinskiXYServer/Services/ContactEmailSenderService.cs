using Microsoft.Extensions.Options;
using System.Net;
using MimeKit;
using System.Net.Mail;
using FluentValidation;
using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Helpers;
using Karpinski_XY_Server.Data.Models.Base;

namespace Karpinski_XY_Server.Services
{
    public class ContactEmailSenderService : Contracts.IContactEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IValidator<ContactDto> _inquiryValidator;
        private readonly ILogger<ContactEmailSenderService> _logger;
        private static readonly Random _random = new Random();

        public ContactEmailSenderService(IOptions<SmtpSettings> smtpSettings,
            IValidator<ContactDto> inquiryValidator,
            ILogger<ContactEmailSenderService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _inquiryValidator = inquiryValidator;
            _logger = logger;
        }

        public async Task<Result<string>> SendEmailAsync(ContactDto inquiry)
        {
            if (IsBlacklisted(inquiry.Email))
            {
                _logger.LogWarning("Email suppressed because it is blacklisted: {Email}", inquiry.Email);
                return Result<string>.Success("Email suppressed (blacklisted)");
            }

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
            messageToRequestor.Bcc.Add(MailboxAddress.Parse(_smtpSettings.CCEmail));
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
        private static string Normalize(string value)
  => value?.Trim().ToLowerInvariant() ?? string.Empty;

        private static string ExtractEmailAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // handles: "Name <user@evil.com>"
            var trimmed = input.Trim();
            var lt = trimmed.LastIndexOf('<');
            var gt = trimmed.LastIndexOf('>');

            if (lt >= 0 && gt > lt)
            {
                trimmed = trimmed.Substring(lt + 1, gt - lt - 1).Trim();
            }

            return trimmed;
        }

        private static string GetDomainSafe(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;

            var at = email.LastIndexOf('@');
            if (at < 0 || at == email.Length - 1) return string.Empty;

            var domain = email.Substring(at + 1).Trim();

            // strip port if present: "evil.com:123"
            var colon = domain.IndexOf(':');
            if (colon > 0) domain = domain.Substring(0, colon);

            // strip trailing punctuation
            domain = domain.Trim().TrimEnd('.', ',', ';');

            return domain;
        }

        private static string NormalizeBlacklistEntry(string entry)
        {
            var normalized = Normalize(entry);

            // strip inline comments: "@evil.com # old"
            var hash = normalized.IndexOf('#');
            if (hash >= 0) normalized = normalized.Substring(0, hash).Trim();

            // remove trailing separators
            normalized = normalized.TrimEnd(',', ';');

            return normalized;
        }

        private static bool IsEmailEntry(string entry)
        {
            var at = entry.IndexOf('@');
            return at > 0 && at == entry.LastIndexOf('@'); // basic sanity
        }

        private bool IsBlacklisted(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var extractedEmail = Normalize(ExtractEmailAddress(email));
            var domain = GetDomainSafe(extractedEmail);
            if (string.IsNullOrWhiteSpace(domain)) return false;

            return _smtpSettings.BlacklistedEmails.Any(rawEntry =>
            {
                var entry = NormalizeBlacklistEntry(rawEntry);
                if (string.IsNullOrWhiteSpace(entry)) return false;

                // exact email match
                if (IsEmailEntry(entry))
                {
                    return entry == extractedEmail;
                }

                // domain match: supports "evil.com" or "@evil.com"
                var blockedDomain = entry.TrimStart('@');

                return domain == blockedDomain || domain.EndsWith("." + blockedDomain);
            });
        }
    }
}
