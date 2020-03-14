using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultFooterScriptChangeMessageTemplate: GetDefaultTemplate<FooterScriptChangeMessageTemplate>
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultFooterScriptChangeMessageTemplate(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        public override async Task<FooterScriptChangeMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            return new FooterScriptChangeMessageTemplate
            {
                Subject = "Footer Script Change - {Status}",
                FromAddress = mailSettings.SystemEmailAddress,
                ToAddress = mailSettings.SystemEmailAddress,
                Body = "The page {Name} ({Url}) has been modified."
            };
        }
    }
}