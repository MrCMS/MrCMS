using MrCMS.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class CKEditorControllerBuilder
    {
        private SiteSettings _siteSettings = new SiteSettings();

        public CKEditorController Build()
        {
            return new CKEditorController(_siteSettings);
        }

        public CKEditorControllerBuilder WithSettings(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
            return this;
        }
    }
}