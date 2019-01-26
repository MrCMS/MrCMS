using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class FileSystemSettingsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var configurationProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IConfigurationProvider>();
            //var modelBinder = new (typeof(FileSystemSettings),serviceProvider.GetRequiredService<ILoggerFactory>());
            //bindingContext.Model = serviceProvider.GetRequiredService<FileSystemSettings>();
            //return modelBinder.BindModelAsync(bindingContext);
            var settings = configurationProvider.GetSiteSettings<FileSystemSettings>();
            var propertyInfos =
                settings.GetType()
                        .GetProperties()
                        .Where(
                            info =>
                            info.CanWrite &&
                            !info.GetCustomAttributes(typeof(ReadOnlyAttribute), true)
                                .Any(o => o.To<ReadOnlyAttribute>().IsReadOnly));

            foreach (var propertyInfo in propertyInfos)
            {
                var result = GetValue(propertyInfo, bindingContext, propertyInfo.Name);

                if (result.Exists)
                    propertyInfo.SetValue(settings, result.Value, null);
            }

            bindingContext.Result = ModelBindingResult.Success(settings);
            return Task.CompletedTask;
        }

        private ValueResult GetValue(PropertyInfo propertyInfo, ModelBindingContext bindingContext, string name)
        {
            var result = bindingContext.ValueProvider.GetValue(name);
            if (result == ValueProviderResult.None)
                return new ValueResult();
            var value = (propertyInfo.PropertyType == typeof(bool)
                ? (object)result.Contains("true")
                : result.FirstValue).ToString();

            return new ValueResult
            {
                Exists = true,
                Value = propertyInfo.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(value)
            };
        }
        private class ValueResult
        {
            public bool Exists { get; set; }
            public object Value { get; set; }
        }

    }

}