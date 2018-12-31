using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Services.ImportExport
{
    public class GetDefaultExportDocumentsEmailTemplate : GetDefaultTemplate<ExportDocumentsEmailTemplate>
    {
        private readonly Site _site;
        private readonly MailSettings _mailSettings;

        public GetDefaultExportDocumentsEmailTemplate(Site site, MailSettings mailSettings)
        {
            _site = site;
            _mailSettings = mailSettings;
        }

        public override ExportDocumentsEmailTemplate Get()
        {
            return new ExportDocumentsEmailTemplate
            {
                IsHtml = true,
                Subject = _site.Name + " Document Export",
                Body = "Please find attached the site document export",
                FromName = "Website",
                FromAddress = _mailSettings.SystemEmailAddress,
            };
        }
    }
}