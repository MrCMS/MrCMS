using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultTrackingScriptsChangeMessageTemplate : GetDefaultTemplate<TrackingScriptsChangeMessageTemplate>
    {
        private readonly MailSettings _mailSettings;

        public GetDefaultTrackingScriptsChangeMessageTemplate(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public override Task<TrackingScriptsChangeMessageTemplate> Get()
        {
            return Task.FromResult(new TrackingScriptsChangeMessageTemplate
            {
                Subject = "Tracking Script Change - {Status}",
                FromAddress = _mailSettings.SystemEmailAddress,
                ToAddress = _mailSettings.SystemEmailAddress,
                Body = "The tracking script has been modified."
            });
        }
    }
}