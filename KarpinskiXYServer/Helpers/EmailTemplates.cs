using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Helpers
{
    public static class EmailTemplates
    {
        public static string RequestorConfirmationTemplate(ContactDto inquiry)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, "resources", "emailtemplates", "index.html");
            string template = File.ReadAllText(filePath);
            var replacedName = template.Replace("{{name}}", inquiry.Name);
            var replacedPhoneNumber = replacedName.Replace("{{phoneNumber}}", inquiry.PhoneNumber);
            var replacedContent = replacedPhoneNumber.Replace("{{content}}", inquiry.Content);

            return replacedContent;
        }
    }
}
