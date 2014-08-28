using System.Web.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class CKEditorController : MrCMSUIController
    {
        private readonly SiteSettings _siteSettings;

        public CKEditorController(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public ContentResult Config()
        {
            return Content(_siteSettings.CKEditorConfig, "application/javascript");
        }
    }
}