using ReadingEmail.Models;

namespace ReadingEmail.Services
{
    public interface IMailService
    {
        List<Mail> GetEmails(string? searchTerm, SearchType? searchType);
        void SendEmail(string to, string subject, string body, string cc);
    }
}
