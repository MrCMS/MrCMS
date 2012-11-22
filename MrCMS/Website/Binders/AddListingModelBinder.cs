using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class AddListingModelBinder : MrCMSDefaultModelBinder
    {
        public AddListingModelBinder(ISession session)
            : base(() => session)
        {
        }
        /*public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = typeof(Listing);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var bindModel = base.BindModel(controllerContext, bindingContext) as Listing;

            bindModel.Widget =
                WidgetHelper.GetNewWidget(controllerContext.HttpContext.Request["WidgetType"]).SetValues(
                    controllerContext.HttpContext.Request.Form);

            return bindModel;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return new Listing();
        }*/
    }
}