using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class ContactController : ApiController
    {
        private readonly IContactEmailSenderService _contactEmailSender;

        public ContactController(IContactEmailSenderService contactEmailSender)
        {
            _contactEmailSender = contactEmailSender;
        }

        [HttpPost]
        [Route("", Name = "registerContact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterInquiryEmail([FromBody] ContactDto contact)
        {
            var result = await _contactEmailSender.SendEmailAsync(contact);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }
    }
}
