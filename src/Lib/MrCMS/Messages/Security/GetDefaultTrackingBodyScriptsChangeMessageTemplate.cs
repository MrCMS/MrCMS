using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultTrackingBodyScriptsChangeMessageTemplate : GetDefaultTemplate<TrackingScriptsBodyChangeMessageTemplate>
    {
        private readonly MailSettings _mailSettings;

        public GetDefaultTrackingBodyScriptsChangeMessageTemplate(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public override TrackingScriptsBodyChangeMessageTemplate Get()
        {
            return new TrackingScriptsBodyChangeMessageTemplate
            {
                Subject = "Tracking Body Script Change - {Status}",
                FromAddress = _mailSettings.SystemEmailAddress,
                ToAddress = _mailSettings.SystemEmailAddress,
                Body = "The body tracking script has been modified."
            };
        }
    }
}