using System.Threading.Tasks;
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
        public override Task<HeaderScriptChangeMessageTemplate> Get()
        {
            return Task.FromResult(new HeaderScriptChangeMessageTemplate
            {
                Subject = "Header Script Change - {Status}",
                FromAddress = _mailSettings.SystemEmailAddress,
                ToAddress = _mailSettings.SystemEmailAddress,
                Body = "The page {Name} ({Url}) has been modified."
            });
        }
    }
}