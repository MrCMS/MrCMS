using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class GetUrlGeneratorOptionsTypeModelBinder:MrCMSDefaultModelBinder
    {
        public GetUrlGeneratorOptionsTypeModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var typeName = GetValueFromContext(controllerContext, "type");
            return TypeHelper.GetTypeByName(typeName);
        }
    }
}