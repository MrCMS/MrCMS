using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class AddUserModelBinder : IModelBinder
    {
        private readonly IModelBinder _worker;

        public AddUserModelBinder(IModelBinder worker)
        {
            _worker = worker;
        }
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _worker.BindModelAsync(bindingContext);// base.BindModel(controllerContext, bindingContext) as User;

            var passwordManagementService =
                bindingContext.HttpContext.RequestServices.GetRequiredService<IPasswordManagementService>();

            passwordManagementService.SetPassword(bindingContext.Result.Model as User, 
                                                   bindingContext.ValueProvider.GetValue("Password").FirstValue,
                                                   bindingContext.ValueProvider.GetValue("ConfirmPassword").FirstValue);
        }
    }
}