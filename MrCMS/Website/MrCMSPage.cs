using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IConfigurationProvider _configurationProvider;
        private ISiteService _siteService;

        public T SiteSettings<T>() where T : SiteSettingsBase, new()
        {
            return _configurationProvider.GetSettings<T>(_siteService.GetCurrentSite());
        }
        public T GlobalSettings<T>() where T : GlobalSettingsBase, new()
        {
            return _configurationProvider.GetSettings<T>();
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            if (MrCMSApplication.DatabaseIsInstalled)
            {
                _siteService = MrCMSApplication.Get<ISiteService>();
                _configurationProvider = MrCMSApplication.Get<IConfigurationProvider>();
            }
        }

    }

    public abstract class MrCMSPage : MrCMSPage<dynamic>
    {
    }
}