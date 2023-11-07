using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class InquiryController : ApiController
    {
        private readonly IinquiryEmailSenderService _inquiryEmailSender;

        public InquiryController(IinquiryEmailSenderService inquiryEmailSender)
        {
            _inquiryEmailSender = inquiryEmailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Registerinquiry(InquiryDto inquiry)
        {
            await _inquiryEmailSender.SendEmailAsync(inquiry);
            return Ok();
        }
    }
}
