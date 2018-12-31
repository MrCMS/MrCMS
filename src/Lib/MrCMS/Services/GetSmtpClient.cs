using System.Net;
using System.Net.Mail;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetSmtpClient : IGetSmtpClient
    {
        public SmtpClient GetClient(MailSettings settings)
        {
            return new SmtpClient(settings.Host, settings.Port)
            {
                EnableSsl = settings.UseSSL,
                Credentials = new NetworkCredential(settings.UserName, settings.Password)
            };
        }
    }
}