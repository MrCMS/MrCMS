using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class GetDefaultHeaderScriptChangeMessageTemplate : GetDefaultTemplate<HeaderScriptChangeMessageTemplate>
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultHeaderScriptChangeMessageTemplate(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override async Task<HeaderScriptChangeMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            return new HeaderScriptChangeMessageTemplate
            {
                Subject = "Header Script Change - {Status}",
                FromAddress = mailSettings.SystemEmailAddress,
                ToAddress = mailSettings.SystemEmailAddress,
                Body = "The page {Name} ({Url}) has been modified."
            };
        }
    }
}