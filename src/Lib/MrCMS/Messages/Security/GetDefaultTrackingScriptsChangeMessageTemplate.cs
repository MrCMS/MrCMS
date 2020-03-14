using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultTrackingScriptsChangeMessageTemplate : GetDefaultTemplate<TrackingScriptsChangeMessageTemplate>
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultTrackingScriptsChangeMessageTemplate(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        public override async Task<TrackingScriptsChangeMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            return new TrackingScriptsChangeMessageTemplate
            {
                Subject = "Tracking Script Change - {Status}",
                FromAddress = mailSettings.SystemEmailAddress,
                ToAddress = mailSettings.SystemEmailAddress,
                Body = "The tracking script has been modified."
            };
        }
    }
}