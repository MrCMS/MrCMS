using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Services.ImportExport
{
    public class GetDefaultExportDocumentsEmailTemplate : GetDefaultTemplate<ExportDocumentsEmailTemplate>
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultExportDocumentsEmailTemplate(IGetCurrentSite getCurrentSite, ISystemConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public override async Task<ExportDocumentsEmailTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var site = await _getCurrentSite.GetSite();
            return new ExportDocumentsEmailTemplate
            {
                IsHtml = true,
                Subject = site.Name + " Document Export",
                Body = "Please find attached the site document export",
                FromName = "Website",
                FromAddress = mailSettings.SystemEmailAddress,
            };
        }
    }
}