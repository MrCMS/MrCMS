using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public abstract class WebpageModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDocumentTagsUpdateService _documentTagsUpdateService;

        protected WebpageModelBinder(IKernel kernel, IDocumentTagsUpdateService documentTagsUpdateService)
            : base(kernel)
        {
            _documentTagsUpdateService = documentTagsUpdateService;
        }

        protected IDocumentTagsUpdateService DocumentTagsUpdateService
        {
            get { return _documentTagsUpdateService; }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;
            string taglist = controllerContext.GetValueFromRequest("TagList") ?? string.Empty;
            DocumentTagsUpdateService.SetTags(taglist, document);
            return document;
        }
    }
}