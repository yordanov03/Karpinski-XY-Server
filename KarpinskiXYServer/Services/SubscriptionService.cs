using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.Extensions.Options;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MailChimp.Net.Core;

namespace Karpinski_XY_Server.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly string _apiKey;
        private readonly string _listId;
        private readonly IMailChimpManager _mailChimpManager;

        public SubscriptionService(IOptions<MailchimpSettings> mailchimpSettings)
        {
            _apiKey = mailchimpSettings.Value.ApiKey;
            _listId = mailchimpSettings.Value.ListId;
            _mailChimpManager = new MailChimpManager(_apiKey);
        }
        public async Task<Result<string>> AddSubscriberAsync(string email)
        {
            try
            {
                var member = new Member
                {
                    EmailAddress = email,
                };

                await _mailChimpManager.Members.AddOrUpdateAsync(_listId, member);

                return Result<string>.Success("Subscriber added successfully.");
            }
            catch (MailChimpException ex)
            {
                return Result<string>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<string>.Fail(ex.Message);
            }
        }
    }
    
}
