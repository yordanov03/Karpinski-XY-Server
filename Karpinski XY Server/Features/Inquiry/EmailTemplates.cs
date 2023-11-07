using Karpinski_XY_Server.Features.Inquiry.Models;

namespace Karpinski_XY_Server.Features.inquiry
{
    public static class EmailTemplates
    {
        public static string RequestorConfirmationTemplate(InquiryDto inquiry)
        {
            string template =  File.ReadAllText("..\\Karpinski XY Server\\Features\\Inquiry\\EmailTemplates\\index.html");
            var replacedName =  template.Replace("{{name}}", inquiry.Name);
            var replacedPhoneNumber = replacedName.Replace("{{phoneNumber}}", inquiry.PhoneNumber);
            var replacedContent = replacedPhoneNumber.Replace("{{content}}", inquiry.Content);

            return replacedContent;
        }
    }
}
