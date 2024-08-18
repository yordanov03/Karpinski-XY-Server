using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Helpers
{
    public static class EmailTemplates
    {
        public static string RequestorConfirmationTemplate(ContactDto inquiry)
        {
            string template = File.ReadAllText("..\\Karpinski XY Server\\Resources\\EmailTemplates\\index.html");
            var replacedName = template.Replace("{{name}}", inquiry.Name);
            var replacedPhoneNumber = replacedName.Replace("{{phoneNumber}}", inquiry.PhoneNumber);
            var replacedContent = replacedPhoneNumber.Replace("{{content}}", inquiry.Content);

            return replacedContent;
        }
    }
}
