using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Areas.Admin.ModelBinders
{
    public class CommentApprovalModelBinder : MrCMSDefaultModelBinder
    {
        public CommentApprovalModelBinder(ISession session)
            : base(() => session)
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