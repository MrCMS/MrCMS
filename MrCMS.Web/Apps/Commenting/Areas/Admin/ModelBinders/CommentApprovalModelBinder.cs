using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Website.Binders;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Commenting.Areas.Admin.ModelBinders
{
    public class CommentApprovalModelBinder : MrCMSDefaultModelBinder
    {
        public CommentApprovalModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is Comment)
            {
                if (controllerContext.HttpContext.Request.Form["Approve"] != null)
                {
                    (model as Comment).Approved = true;
                }
                if (controllerContext.HttpContext.Request.Form["Reject"] != null)
                {
                    (model as Comment).Approved = false;
                }
            }
            return model;
        }
    }
}