using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class SettingModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var settingTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<ISettings>();
            var objects = settingTypes.Select(type =>
                                                  {
                                                      var configProvider = typeof(ConfigurationProvider<>);
                                                      var genericType = configProvider.MakeGenericType(type);

                                                      var configurationProvider =
                                                          Activator.CreateInstance(genericType,
                                                                                   MrCMSApplication.Get<ISettingService>
                                                                                       ());

                                                      return
                                                          configurationProvider.GetType()
                                                                               .GetProperty("Settings")
                                                                               .GetValue(configurationProvider, null);
                                                  }).OfType<ISettings>().ToList();

            foreach (var settings in objects)
            {
                var propertyInfos =
                    settings.GetType()
                            .GetProperties()
                            .Where(
                                info =>
                                info.CanWrite &&
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
            /*
            var propertyNames =
                _settingTypes.SelectMany(
                    type =>
                    type.GetProperties()
                        .Where(info => info.CanWrite)
                        .Select(property => (type.FullName + "." + property.Name).ToLower()));

            var objects = _settingTypes.Select(type =>
            {
                var configProvider = typeof(ConfigurationProvider<>);
                var genericType = configProvider.MakeGenericType(type);

                var configurationProvider = Activator.CreateInstance(genericType, _settingService);

                return
                    configurationProvider.GetType()
                                         .GetProperty("Settings")
                                         .GetValue(configurationProvider, null);
            }).OfType<ISettings>();

            foreach (var o in objects)
            {
                var keys = collection.AllKeys.Where(s => s.StartsWith(o.GetType().FullName, StringComparison.OrdinalIgnoreCase));

                var valueCollection = new NameValueCollection();
                foreach (var key in keys)
                    valueCollection.Add(key.Split('.').Last(), collection[key]);

                var memberInfos =
                    GetType()
                        .GetMember("TryUpdateModel", MemberTypes.Method,
                                   BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                   BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                        .OfType<MethodInfo>();

                var tryUpdateModel =
                    memberInfos.FirstOrDefault(
                        info =>
                        info.GetParameters().Length == 2 &&
                        info.GetParameters()[1].ParameterType == typeof(IValueProvider));

                var result = tryUpdateModel.MakeGenericMethod(o.GetType()).Invoke(this, new object[] { o, new FormCollection(valueCollection) });

                var hashCode = result.GetHashCode();
                var clone = memberInfos.ToList();
            }

            foreach (var propertyName in propertyNames)
                _settingService.SetSetting(propertyName, collection[propertyName]);
            */
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