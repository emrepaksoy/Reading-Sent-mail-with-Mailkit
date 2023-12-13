namespace ReadingEmail.Models
{
    public class MailConfig
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string ImapHost { get; set; }
        public int ImapPort { get; set; }
        public string UserName { get; set; }
        public string AppPassword { get; set; }

    }
}
