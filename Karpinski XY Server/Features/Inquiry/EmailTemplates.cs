namespace Karpinski_XY_Server.Features.inquiry
{
    public static class EmailTemplates
    {
        public static string RequestorConfirmationTemplate(string name, int caseId)
        {
            return $"Dear {name},\n" +
                $"Thank you so much for getting in touch with me!For tracking purposes I have assisgned your inquiry a number: {caseId} and will get back to you at earliest convenience.\n" +
                "You will be charged 1 kissy per every charachter in thi message\n" +
                $"Best Regards,\n" +
                $"Pawel";
        }


        public static string NotificationToArtist(inquiryDto inquiry, int caseId)
        {
            return "Dear Pawlisuhko\n" +
                "A new inquiry has been submitted to you with the following information:\n" +
                $"case: {caseId}\n" +
                $"name: {inquiry.Name}\n" +
                $"email: {inquiry.Email}\n" +
                $"phone number: {inquiry.PhoneNumber}\n" +
                $"subject: {inquiry.Subject}\n" +
                $"content: {inquiry.Content}\n" +
                "A confirmation email has been sent as a result of the inquiry to the inquerer\n" +
                "Best Regards\n" +
                "Your admin Svetliushko";

        }
    }
}
