namespace Karpinski_XY_Server.Features.Inquery
{
    public static class EmailTemplates
    {
        public static string RequestorConfirmationTemplate(string name, int caseId)
        {
            return $"Dear {name},\n" +
                $"Thank you so much for getting in touch with me!For tracking purposes I have assisgned your inquery a number: {caseId} and will get back to you at earliest convenience.\n" +
                $"Best Regards,\n" +
                $"Pawel";
        }


        public static string NotificationToArtist(InqueryDto inquery, int caseId)
        {
            return "Dear Pawlisuhko\n" +
                "A new inquery has been submitted to you with the following information:\n" +
                $"case: {caseId}\n" +
                $"name: {inquery.Name}\n" +
                $"email: {inquery.Email}\n" +
                $"phone number: {inquery.PhoneNumber}\n" +
                $"subject: {inquery.Subject}\n" +
                $"content: {inquery.Content}\n" +
                "A confirmation email has been sent as a result of the inquery to the inquerer\n" +
                "Best Regards\n" +
                "Your admin Svetliushko";

        }
    }
}
