using System.Net.Mail;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface IGetSmtpClient
    {
        Task<SmtpClient> GetClient();
        SmtpClient GetClient(MailSettings settings);
    }
}