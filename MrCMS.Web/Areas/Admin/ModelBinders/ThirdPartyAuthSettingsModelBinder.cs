using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class ThirdPartyAuthSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public ThirdPartyAuthSettingsModelBinder(IKernel kernel, ISystemConfigurationProvider configurationProvider) : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
        {
            return _configurationProvider.GetSystemSettings<ThirdPartyAuthSettings>();
        }
    }
}