using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class InquiryController : ApiController
    {
        private readonly IInquiryEmailSenderService _inquiryEmailSender;

        public InquiryController(IInquiryEmailSenderService inquiryEmailSender)
        {
            _inquiryEmailSender = inquiryEmailSender;
        }

        [HttpPost]
        [Route("", Name = "registerInquiry")]
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
