using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IConfigurationProvider _configurationProvider;
        private ISiteService _siteService;

        public T Settings<T>() where T : ISettings, new()
        {
            return _configurationProvider.GetSettings<T>(_siteService.GetCurrentSite());
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