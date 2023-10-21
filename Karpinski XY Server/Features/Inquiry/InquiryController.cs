using Karpinski_XY.Features;
using Karpinski_XY_Server.Features.Inquiry.Models;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Features.inquiry
{
    public class InquiryController : ApiController
    {
        private readonly IInquiryEmailSenderService _inquiryEmailSender;

        public InquiryController(IInquiryEmailSenderService inquiryEmailSender)
        {
            this._inquiryEmailSender = inquiryEmailSender;
        }

        [HttpPost]
        [Route("", Name ="registerInquiry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterInquiryEmail([FromBody] InquiryDto inquiry)
        {
            var result = await _inquiryEmailSender.SendEmailAsync(inquiry);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }
    }
}
