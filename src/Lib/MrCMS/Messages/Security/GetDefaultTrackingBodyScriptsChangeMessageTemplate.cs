using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultTrackingBodyScriptsChangeMessageTemplate : GetDefaultTemplate<TrackingScriptsBodyChangeMessageTemplate>
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultTrackingBodyScriptsChangeMessageTemplate(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override async Task<TrackingScriptsBodyChangeMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            return new TrackingScriptsBodyChangeMessageTemplate
            {
                Subject = "Tracking Body Script Change - {Status}",
                FromAddress = mailSettings.SystemEmailAddress,
                ToAddress = mailSettings.SystemEmailAddress,
                Body = "The body tracking script has been modified."
            };
        }
    }
}