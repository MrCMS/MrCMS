using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public abstract class WebpageModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDocumentTagsAdminService _documentTagsAdminService;

        protected WebpageModelBinder(IKernel kernel, IDocumentTagsAdminService documentTagsAdminService)
            : base(kernel)
        {
            _documentTagsAdminService = documentTagsAdminService;
        }

        protected IDocumentTagsAdminService DocumentTagsAdminService
        {
            get { return _documentTagsAdminService; }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;
            string taglist = controllerContext.GetValueFromRequest("TagList") ?? string.Empty;
            DocumentTagsAdminService.SetTags(taglist, document);
            return document;
        }
    }
}