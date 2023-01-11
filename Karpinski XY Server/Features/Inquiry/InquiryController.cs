using Karpinski_XY.Features;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Features.inquiry
{
    public class inquiryController : ApiController
    {
        private readonly IinquiryEmailSenderService _inquiryEmailSender;

        public inquiryController(IinquiryEmailSenderService inquiryEmailSender)
        {
            this._inquiryEmailSender = inquiryEmailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Registerinquiry(inquiryDto inquiry)
        {
            var result = await this._inquiryEmailSender.SendEmailAsync(inquiry);
            return Ok();
        }
    }
}
