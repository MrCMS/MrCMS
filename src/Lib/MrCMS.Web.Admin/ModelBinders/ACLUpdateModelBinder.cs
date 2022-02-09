using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Models;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class ACLUpdateModelBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nameValueCollection = bindingContext.HttpContext.Request.Form;

            var keys = nameValueCollection.Keys.Where(s => s.StartsWith("acl-"));

            bindingContext.Result = ModelBindingResult.Success(keys.Select(s =>
            {
                var substring = s.Substring(4).Split('-');
                return new ACLUpdateRecord
                {
                    Role = substring[0],
                    Key = substring[1],
                    Allowed = nameValueCollection[s].Contains("true")
                };
            }).ToList());

            return Task.CompletedTask;
        }
    }
}