using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class UpdateUserRoleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var roles = new List<int>();

            IFormCollection form = bindingContext.HttpContext.Request.Form;
            IEnumerable<string> keys = form.Keys.Where(s => s.StartsWith("Role-"));

            foreach (string key in keys)
            {
                var value = form[key];
                string idVal = key.Substring(5);
                if (int.TryParse(idVal, out int id) && value.Any(x => x.Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    roles.Add(id);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(roles);
            return Task.CompletedTask;
        }
    }
}