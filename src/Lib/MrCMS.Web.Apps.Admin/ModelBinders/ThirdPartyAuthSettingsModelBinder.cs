using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    //public class ThirdPartyAuthSettingsModelBinder : IModelBinder
    //{
    //    public Task BindModelAsync(ModelBindingContext bindingContext)
    //    {
    //        var serviceProvider = bindingContext.HttpContext.RequestServices;
    //        bindingContext.Model = serviceProvider .GetRequiredService<ISystemConfigurationProvider>().GetSystemSettings<ThirdPartyAuthSettings>();

    //        var form = bindingContext.HttpContext.Request.Form;

    //        foreach (var property in typeof(ThirdPartyAuthSettings).GetProperties().Where(x=>x.CanWrite))
    //        {
    //            if(!form.ContainsKey(property.Name))
    //                continue;

    //            var value = GetValue(property, bindingContext, property.Name);
    //            property.SetValue(bindingContext.Model, value);
    //        }

    //        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

    //        return Task.CompletedTask;
    //    }

    //    private object GetValue(PropertyInfo propertyInfo, ModelBindingContext bindingContext, string fullName)
    //    {
    //        var value = (propertyInfo.PropertyType == typeof(bool)
    //            ? (object)bindingContext.HttpContext.Request.Form[fullName].Contains("true")
    //            : bindingContext.HttpContext.Request.Form[fullName]).ToString();

    //        return propertyInfo.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(value);
    //    }
    //}
}