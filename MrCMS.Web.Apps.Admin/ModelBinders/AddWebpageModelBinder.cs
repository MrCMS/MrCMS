using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class AddWebpageModelBinder : IModelBinder
    {


        private static (Type webpageType, int? pageTemplateId) GetTypeByName(ModelBindingContext bindingContext)
        {
            string valueFromContext = bindingContext.ValueProvider.GetValue("DocumentType").FirstValue;
            var strings = valueFromContext.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length == 1)
                return
                    (TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == valueFromContext),
                        null);
            int id;
            return (TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == strings[0]),
                int.TryParse(strings[1], out id) ? id : (int?)null);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeInfo = GetTypeByName(bindingContext);
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(typeInfo.webpageType);
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, typeof(SystemEntityBinder<>).MakeGenericType(typeInfo.webpageType)) as IModelBinder;

            await instance.BindModelAsync(bindingContext);

            //set include as navigation as default 
            if (bindingContext.Result.Model is Webpage webpage)
            {
                if (typeInfo.pageTemplateId.HasValue)
                {
                    webpage.PageTemplate = serviceProvider.GetRequiredService<ISession>().Get<PageTemplate>(typeInfo.pageTemplateId.Value);
                }
                webpage.RevealInNavigation = webpage.GetMetadata().RevealInNavigation;

                var tagAdminService = serviceProvider.GetRequiredService<IDocumentTagsUpdateService>();
                string taglist = bindingContext.ValueProvider.GetValue("TagList").FirstValue ?? string.Empty;
                tagAdminService.SetTags(taglist, webpage);
            }
        }
    }
}