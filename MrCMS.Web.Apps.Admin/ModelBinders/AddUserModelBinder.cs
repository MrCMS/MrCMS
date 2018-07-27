using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class AddUserModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var systemEntityBinder = new SystemEntityBinder<User>(bindingContext.HttpContext.RequestServices.GetRequiredService<ISession>());
            await systemEntityBinder.BindModelAsync(bindingContext);// base.BindModel(controllerContext, bindingContext) as User;

            var passwordManagementService =
                bindingContext.HttpContext.RequestServices.GetRequiredService<IPasswordManagementService>();

            passwordManagementService.SetPassword(bindingContext.Result.Model as User, 
                                                   bindingContext.ValueProvider.GetValue("Password").FirstValue,
                                                   bindingContext.ValueProvider.GetValue("ConfirmPassword").FirstValue);
        }
    }
}