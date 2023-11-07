﻿using Karpinski_XY_Server.Controllers;
using Karpinski_XY_Server.Features.Inquiry.Models;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Features.inquiry
{
    public class InquiryController : ApiController
    {
        private readonly IinquiryEmailSenderService _inquiryEmailSender;

        public InquiryController(IinquiryEmailSenderService inquiryEmailSender)
        {
            this._inquiryEmailSender = inquiryEmailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Registerinquiry(InquiryDto inquiry)
        {
            await this._inquiryEmailSender.SendEmailAsync(inquiry);
            return Ok();
        }
    }
}
