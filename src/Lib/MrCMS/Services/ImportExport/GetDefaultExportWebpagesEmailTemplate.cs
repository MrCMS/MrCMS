using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Services.ImportExport
{
    public class GetDefaultExportWebpagesEmailTemplate : GetDefaultTemplate<ExportWebpagesEmailTemplate>
    {
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultExportWebpagesEmailTemplate(ICurrentSiteLocator siteLocator, MailSettings mailSettings)
        {
            _siteLocator = siteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<ExportWebpagesEmailTemplate> Get()
        {
            var site = _siteLocator.GetCurrentSite();
            return Task.FromResult(new ExportWebpagesEmailTemplate
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