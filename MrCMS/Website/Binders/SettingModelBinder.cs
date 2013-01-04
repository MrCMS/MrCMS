using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using Ninject;
using MrCMS.Entities.Multisite;

namespace MrCMS.Website.Binders
{
    public class SettingModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var settingTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<ISettings>();
            var sitesService = MrCMSApplication.Get<ISiteService>();
            // Uses Id because the settings are edited on the same page as the site itself
            var siteId = controllerContext.HttpContext.Request["Id"];

            var objects = settingTypes.Select(type =>
                                                  {
                                                      var configurationProvider = MrCMSApplication.Get<ConfigurationProvider>();

                                                      var methodInfo = typeof(ConfigurationProvider).GetMethodExt("GetSettings", typeof(Site));

                                                      return
                                                          methodInfo.MakeGenericMethod(type).Invoke(configurationProvider,
                                                                            new object[]
                                                                                {
                                                                                    sitesService.GetSite(
                                                                                        Convert.ToInt32(siteId))
                                                                                });
                                                  }).OfType<ISettings>().ToList();

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
    }
}