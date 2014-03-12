using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddPropertiesModelBinder : MrCMSDefaultModelBinder
    {
        public AddPropertiesModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var typeName = GetValueFromContext(controllerContext, "type");
            var webpageType = TypeHelper.GetMappedClassesAssignableFrom<Webpage>().FirstOrDefault(type => type.FullName == typeName);

            if (webpageType != null && webpageType.HasDefaultConstructor())
            {
                return Activator.CreateInstance(webpageType);
            }
            return null;
        }
    }
}