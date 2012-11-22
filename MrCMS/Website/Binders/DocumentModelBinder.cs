using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Website.Binders
{
    public abstract class DocumentModelBinder : MrCMSDefaultModelBinder
    {
        protected readonly IDocumentService DocumentService;

        protected DocumentModelBinder(ISession session, IDocumentService documentService)
            : base(() => session)
        {
            this.DocumentService = documentService;
        }
    }
}