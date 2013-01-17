using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class InPageAdminController : AdminController
    {
        private readonly IDocumentService _documentService;

        public InPageAdminController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var id = filterContext.RouteData.Values["id"];
            int idVal;
            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)) && int.TryParse(Convert.ToString(id), out idVal))
            {
                var document = _documentService.GetDocument<Webpage>(idVal);
                if (document != null && !document.IsAllowedForAdmin(MrCMSApplication.CurrentUser))
                {
                    filterContext.Result = new EmptyResult();
                }
            }
        }

        public PartialViewResult InPageEditor(Webpage page)
        {
            return PartialView("InPageEditor", page);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void SaveBodyContent(string content, int id)
        {
            var doc = _documentService.GetDocument<TextPage>(id);
            doc.BodyContent = content;
            _documentService.SaveDocument(doc);
        }
    }
}