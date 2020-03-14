using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetSmtpClient : IGetSmtpClient
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetSmtpClient(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<SmtpClient> GetClient()
        {
            var settings = await _configurationProvider.GetSystemSettings<MailSettings>();
            return GetClient(settings);
        }

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