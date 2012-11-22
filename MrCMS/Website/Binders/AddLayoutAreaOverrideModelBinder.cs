using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class AddLayoutAreaOverrideModelBinder : MrCMSDefaultModelBinder
    {
        public AddLayoutAreaOverrideModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = typeof(LayoutAreaOverride);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var bindModel = base.BindModel(controllerContext, bindingContext) as LayoutAreaOverride;

            bindModel.Widget =
                WidgetHelper.GetNewWidget(controllerContext.HttpContext.Request["WidgetType"]).SetValues(
                    controllerContext.HttpContext.Request.Form);

            return bindModel;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return new LayoutAreaOverride();
        }
    }
}