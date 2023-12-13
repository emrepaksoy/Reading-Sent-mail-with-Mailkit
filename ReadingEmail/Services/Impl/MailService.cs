using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using ReadingEmail.Models;
using System.Net;

namespace ReadingEmail.Services.Impl
{
    public class MailService : IMailService
    {
        public MailConfig MailConfig;

        public MailService(IConfiguration configuration)
        {
            MailConfig = configuration.GetSection("Mails:Gmail").Get<MailConfig>();
        }
        public List<Mail> GetEmails(string? searchTerm, SearchType? searchType )
        {
            var mails = new List<Mail>();
            using (var client = new ImapClient())
            {
                client.Connect(MailConfig.ImapHost, MailConfig.ImapPort, SecureSocketOptions.SslOnConnect);
                client.Authenticate(new NetworkCredential(MailConfig.UserName, MailConfig.AppPassword));

                var sentFolder = client.GetFolder(SpecialFolder.Sent);

                if (sentFolder != null)
                {
                    sentFolder.Open(FolderAccess.ReadOnly);

                    SearchQuery  searchQuery;

                    switch (searchType)
                    {
                        case SearchType.None:
                            searchQuery = SearchQuery.All; 
                            break;
                        case SearchType.To:
                            searchQuery = SearchQuery.ToContains(searchTerm);
                            break;
                        case SearchType.Subject:
                            searchQuery = SearchQuery.SubjectContains(searchTerm);
                            break;
                        case SearchType.Body:
                            searchQuery = SearchQuery.BodyContains(searchTerm);
                            break;
                        default:
                            searchQuery = SearchQuery.All;
                            break;
                    }

                    var messages = sentFolder.Search(searchQuery);

                    mails.AddRange(messages.Select(uniqueId =>
                    {
                        var message = sentFolder.GetMessage(uniqueId);
                        return new Mail
                        {
                            To = message.To.ToString(),
                            Body = message.Body.ToString(),
                            Subject = message.Subject.ToString(),
                            Cc = message.Cc.ToString(),
                        };
                    }).ToList());
                }
                client.Disconnect(true);
            }

            return mails;
        }

        public void SendEmail(string to, string subject, string body, string cc)
        {
            var emailMessage = GetEmailMessage( to, subject, body , cc, MailConfig.UserName);
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(MailConfig.SmtpHost, MailConfig.SmtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(MailConfig.UserName, MailConfig.AppPassword);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }

        private MimeMessage GetEmailMessage(string to, string subject, string body, string cc, string FromName)
        {
            var emailMessage = new MimeMessage();
            emailMessage.Cc.Add(new MailboxAddress("", cc));
            emailMessage.From.Add(new MailboxAddress("Mail test", FromName));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = body };
            return emailMessage;
        }
    }
}
