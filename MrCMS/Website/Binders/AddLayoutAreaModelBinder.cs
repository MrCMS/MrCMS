using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using NHibernate;
using System.Linq;

namespace MrCMS.Website.Binders
{
    public class AddLayoutAreaModelBinder : MrCMSDefaultModelBinder
    {
        public AddLayoutAreaModelBinder(ISession session)
            : base(() => session)
        {
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = typeof(LayoutArea);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var layoutArea = base.BindModel(controllerContext, bindingContext) as LayoutArea;

            return layoutArea;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return new LayoutArea();
        }
    }
}