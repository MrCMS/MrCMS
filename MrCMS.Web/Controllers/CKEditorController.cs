using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
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