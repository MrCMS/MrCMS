using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class EditDocumentModelBinder : DocumentModelBinder
    {
        public EditDocumentModelBinder(ISession session, IDocumentService documentService) : base(session, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;

            var taglist = GetValueFromContext(controllerContext, "TagList");
            DocumentService.SetTags(taglist, document);

            if (document is Webpage)
            {
                var frontEndRoles = GetValueFromContext(controllerContext, "FrontEndRoles");
                DocumentService.SetFrontEndRoles(frontEndRoles, document as Webpage);
            }

            return document;
        }
    }
}