using System.Web.Mvc;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class LoginModelModelBinder : MrCMSDefaultModelBinder
    {
        public LoginModelModelBinder(ISession session)
            : base(() => session)
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