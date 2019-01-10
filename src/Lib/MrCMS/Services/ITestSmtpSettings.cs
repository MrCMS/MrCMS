using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface ITestSmtpSettings
    {
        bool TestSettings(MailSettings settings, TestEmailInfo info);
    }
}