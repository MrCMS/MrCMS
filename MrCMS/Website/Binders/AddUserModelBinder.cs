using System;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class AddUserModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IPasswordManagementService _passwordManagementService;

        public AddUserModelBinder(IPasswordManagementService passwordManagementService, ISession session)
            : base(() => session)
        {
            _passwordManagementService = passwordManagementService;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = base.BindModel(controllerContext, bindingContext) as User;

            _passwordManagementService.SetPassword(user,
                                                   controllerContext.RequestContext.HttpContext.Request["Password"],
                                                   controllerContext.RequestContext.HttpContext.Request[
                                                       "ConfirmPassword"]);

            return user;
        }
    }
}