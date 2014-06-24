using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddUserModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IPasswordManagementService _passwordManagementService;

        public AddUserModelBinder(IKernel kernel, IPasswordManagementService passwordManagementService) : base(kernel)
        {
            _passwordManagementService = passwordManagementService;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = base.BindModel(controllerContext, bindingContext) as User;

            _passwordManagementService.SetPassword(user,
                                                   controllerContext.GetValueFromRequest("Password"),
                                                   controllerContext.GetValueFromRequest("ConfirmPassword"));

            return user;
        }
    }
}