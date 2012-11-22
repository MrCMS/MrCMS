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
        public AddUserModelBinder(IAuthorisationService authorisationService, ISession session)
            : base(() => session)
        {
            AuthorisationService = authorisationService;
        }

        private IAuthorisationService AuthorisationService { get; set; }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = base.BindModel(controllerContext, bindingContext) as User;

            AuthorisationService.SetPassword(user, controllerContext.RequestContext.HttpContext.Request["Password"],
                                             controllerContext.RequestContext.HttpContext.Request["ConfirmPassword"]);

            return user;
        }
    }
}