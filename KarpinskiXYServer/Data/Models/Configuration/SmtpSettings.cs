namespace Karpinski_XY_Server.Data.Models.Configuration
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string CCEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
