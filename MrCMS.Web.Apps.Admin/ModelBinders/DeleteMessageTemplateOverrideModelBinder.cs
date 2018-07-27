using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class DeleteMessageTemplateOverrideModelBinder : IModelBinder
    {
        private static Type GetTypeByName(ModelBindingContext bindingContext)
        {
            string valueFromContext = bindingContext.ValueProvider.GetValue("TemplateType").FirstValue;
            return TypeHelper.GetTypeByName(valueFromContext);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Type type = GetTypeByName(bindingContext);

            var messageTemplateProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IMessageTemplateProvider>();
            var site = bindingContext.HttpContext.RequestServices.GetRequiredService<ICurrentSiteLocator>().GetCurrentSite();
            var model =  messageTemplateProvider.GetAllMessageTemplates(site)
                .FirstOrDefault(template => template.SiteId == site.Id && template.GetType() == type);
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}