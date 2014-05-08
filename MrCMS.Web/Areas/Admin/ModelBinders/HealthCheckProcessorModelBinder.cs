using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class HealthCheckProcessorModelBinder : MrCMSDefaultModelBinder
    {
        public HealthCheckProcessorModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string typeName = GetValueFromContext(controllerContext, "typeName");
            var typeByName = TypeHelper.GetTypeByName(typeName);
            return typeName == null ? null : Kernel.Get(typeByName);
        }
    }
}