using System.Net.Mail;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface IGetSmtpClient
    {
        SmtpClient GetClient(MailSettings settings);
    }
}