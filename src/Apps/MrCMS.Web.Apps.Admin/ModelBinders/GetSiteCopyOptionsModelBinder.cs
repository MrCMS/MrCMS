using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class GetSiteCopyOptionsModelBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var siteCopyOptions = new List<SiteCopyOption>();

            IFormCollection form = bindingContext.HttpContext.Request.Form;
            IEnumerable<string> keys = form.Keys.Where(s => s.StartsWith("sco-"));

            foreach (string key in keys)
            {
                string value = form[key];
                string typeName = key.Substring(4);
                Type type = TypeHelper.GetTypeByName(typeName);
                if (int.TryParse(value, out var id) && type != null)
                {
                    siteCopyOptions.Add(new SiteCopyOption {SiteCopyActionType = type, SiteId = id});
                }
            }

            bindingContext.Result = ModelBindingResult.Success(siteCopyOptions);
            return Task.CompletedTask;
        }
    }
}