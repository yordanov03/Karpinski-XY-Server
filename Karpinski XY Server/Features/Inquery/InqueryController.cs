using Karpinski_XY.Features;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Features.Inquery
{
    public class InqueryController : ApiController
    {
        private readonly IInqueryEmailSenderService _inqueryEmailSender;

        public InqueryController(IInqueryEmailSenderService inqueryEmailSender)
        {
            this._inqueryEmailSender = inqueryEmailSender;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterInquery(InqueryDto inquery)
        {
            var result = await this._inqueryEmailSender.SendEmailAsync(inquery);
            return Ok();
        }
    }
}
