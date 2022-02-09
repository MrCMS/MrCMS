using System.Threading.Tasks;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface ITestSmtpSettings
    {
        Task<bool> TestSettings(MailSettings settings, TestEmailInfo info);
    }
}