using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class SiteSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IConfigurationProvider _configurationProvider;

        public SiteSettingsModelBinder(IKernel kernel, IConfigurationProvider configurationProvider) : base(kernel)
        {
            _configurationProvider = configurationProvider;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var settingTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>();
            // Uses Id because the settings are edited on the same page as the site itself

            var objects = settingTypes.Select(type =>
                                                  {
                                                      var methodInfo = GetGetSettingsMethod();
                                                      return
                                                          methodInfo.MakeGenericMethod(type)
                                                                    .Invoke(_configurationProvider,
                                                                            new object[]
                                                                                {});
                                                  }).OfType<SiteSettingsBase>().Where(arg => arg.RenderInSettings).ToList();

            foreach (var settings in objects)
            {
                var propertyInfos =
                    settings.GetType()
                            .GetProperties()
                            .Where(
                                info =>
                                info.CanWrite &&
                                info.Name != "Site" &&
                                !info.GetCustomAttributes(typeof (ReadOnlyAttribute), true)
                                    .Any(o => o.To<ReadOnlyAttribute>().IsReadOnly));

                foreach (var propertyInfo in propertyInfos)
                {
                    propertyInfo.SetValue(settings,
                                          GetValue(propertyInfo, controllerContext,
                                                   (settings.GetType().FullName + "." + propertyInfo.Name).ToLower()), null);
                }
            }

            return objects;
        }


        private object GetValue(PropertyInfo propertyInfo, ControllerContext controllerContext, string fullName)
        {
            var value = (propertyInfo.PropertyType == typeof(bool)
                             ? (object)controllerContext.HttpContext.Request[fullName].Contains("true")
                             : controllerContext.HttpContext.Request[fullName]).ToString();

            return propertyInfo.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(value);
        }

        protected virtual MethodInfo GetGetSettingsMethod()
        {
            return typeof (ConfigurationProvider).GetMethodExt("GetSiteSettings");
        }
    }
}