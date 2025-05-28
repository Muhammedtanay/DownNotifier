using System.Net.Mail;
using System.Net;

namespace DownNotifier.Business.Services
{
    public class EmailNotificationService : INotificationService
    {
        public async Task NotifyAsync(string to, string subject, string message)
        {
            using var smtp = new SmtpClient("smtp.sunucu.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("kullanici", "şifre"),
                EnableSsl = true
            };

            var mail = new MailMessage("noreply@site.com", to, subject, message);
            await smtp.SendMailAsync(mail);
        }
    }
}