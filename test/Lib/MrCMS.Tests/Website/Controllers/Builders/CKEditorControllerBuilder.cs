using FakeItEasy;
using MrCMS.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class CKEditorControllerBuilder
    {
        private SiteSettings _siteSettings = new SiteSettings();
        private readonly IConfigurationProvider _configurationProvider = A.Fake<IConfigurationProvider>();

        public CKEditorController Build()
        {
            return new CKEditorController(_configurationProvider);
        }

        public CKEditorControllerBuilder WithSettings(SiteSettings siteSettings)
        {
            A.CallTo(() => _configurationProvider.GetSiteSettings<SiteSettings>()).Returns(siteSettings);
            return this;
        }
    }
}