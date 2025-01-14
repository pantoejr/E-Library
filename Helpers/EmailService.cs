using MailKit.Net.Smtp;
using MimeKit;

namespace E_Library.Helpers
{
    public class EmailService
    {
        private readonly string _smtpServer = "mail5018.site4now.net";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "accounts@suclibrary.site";
        private readonly string _smtpPassword = "P@$$w0rd@2025";

        public bool SendEmail(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Electronic Library", _smtpUser));

            message.To.Add(new MailboxAddress("", toEmail));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(_smtpUser, _smtpPassword);
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            { 
                return false;
            }
            finally
            {
                client.Disconnect(true);
            }
        }
    }
}
