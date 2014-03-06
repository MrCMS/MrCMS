using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class ThirdPartyAuthSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ThirdPartyAuthSettingsModelBinder(IConfigurationProvider configurationProvider, ISession session) : base(() => session)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
        {
            return _configurationProvider.GetSiteSettings<ThirdPartyAuthSettings>();
        }
    }
}