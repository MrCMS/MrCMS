using System.Web.Mvc;
using MrCMS.Models.Auth;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website.Binders;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class LoginModelModelBinder : MrCMSDefaultModelBinder
    {
        public LoginModelModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is LoginModel)
            {
                if (!string.IsNullOrWhiteSpace((bindModel as LoginModel).Email))
                {
                    (bindModel as LoginModel).Email = (bindModel as LoginModel).Email.Trim();
                }
            }
            return bindModel;
        }
    }
}