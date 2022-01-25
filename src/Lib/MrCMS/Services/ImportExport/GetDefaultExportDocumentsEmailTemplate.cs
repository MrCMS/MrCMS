using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Services.ImportExport
{
    public class GetDefaultExportDocumentsEmailTemplate : GetDefaultTemplate<ExportDocumentsEmailTemplate>
    {
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultExportDocumentsEmailTemplate(ICurrentSiteLocator siteLocator, MailSettings mailSettings)
        {
            _siteLocator = siteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<ExportDocumentsEmailTemplate> Get()
        {
            var site = _siteLocator.GetCurrentSite();
            return Task.FromResult(new ExportDocumentsEmailTemplate
            {
                IsHtml = true,
                Subject = site.Name + " Document Export",
                Body = "Please find attached the site document export",
                FromName = "Website",
                FromAddress = _mailSettings.SystemEmailAddress,
            });
        }
    }
}