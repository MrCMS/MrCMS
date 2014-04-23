using System.Web;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.ModelBinders
{
    public class SetIPAddressModelBinder : MrCMSDefaultModelBinder
    {
        private readonly HttpContextBase _context;

        public SetIPAddressModelBinder(HttpContextBase context, ISession session)
            : base(() => session)
        {
            _context = context;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            
            if (bindModel is IHaveIPAddress)
                (bindModel as IHaveIPAddress).IPAddress = _context.GetCurrentIP();

            return bindModel;
        }
    }
}