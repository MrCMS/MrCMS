using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class SystemSettingsModelBinder : IModelBinder
    {
        private object GetValue(PropertyInfo propertyInfo, ModelBindingContext bindingContext, string fullName)
        {
            var value = (propertyInfo.PropertyType == typeof(bool)
                ? (object)bindingContext.HttpContext.Request.Form[fullName].Contains("true")
                : bindingContext.HttpContext.Request.Form[fullName]).ToString();

            return propertyInfo.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(value);
        }

        protected virtual MethodInfo GetGetSettingsMethod()
        {
            return typeof(SqlSystemConfigurationProvider).GetMethodExt(nameof(SqlSystemConfigurationProvider
                .GetSystemSettings));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var settingTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>();
            // Uses Id because the settings are edited on the same page as the site itself
            var configurationProvider =
                bindingContext.HttpContext.RequestServices.GetRequiredService<ISystemConfigurationProvider>();

            var objects = settingTypes.Select(type =>
            {
                var methodInfo = GetGetSettingsMethod();
                return
                    methodInfo.MakeGenericMethod(type)
                        .Invoke(configurationProvider,
                            new object[]
                            {});
            }).OfType<SystemSettingsBase>().Where(arg => arg.RenderInSettings).ToList();

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
                        GetValue(propertyInfo, bindingContext,
                            (settings.GetType().FullName + "." + propertyInfo.Name).ToLower()), null);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(objects);
            return Task.CompletedTask;
        }
    }
}