using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultHeaderScriptChangeMessageTemplate : GetDefaultTemplate<HeaderScriptChangeMessageTemplate>
    {
        private readonly MailSettings _mailSettings;

        public GetDefaultHeaderScriptChangeMessageTemplate(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public override HeaderScriptChangeMessageTemplate Get()
        {
            return new HeaderScriptChangeMessageTemplate
            {
                Subject = "Header Script Change - {Status}",
                FromAddress = _mailSettings.SystemEmailAddress,
                ToAddress = _mailSettings.SystemEmailAddress,
                Body = "The page {Name} ({Url}) has been modified."
            };
        }
    }
}