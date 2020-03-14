using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class CKEditorController : MrCMSUIController
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CKEditorController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<ContentResult> Config()
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            return Content(siteSettings.CKEditorConfig, "application/javascript");
        }
    }
}