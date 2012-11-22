using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using Ninject;

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

            return document;
        }
    }
}