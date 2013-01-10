using System;
using System.IO;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    [Authorize(Roles = UserRole.Administrator)]
    public class AdminToolsController : MrCMSController
    {
        private readonly IDocumentService _documentService;

        public AdminToolsController(IDocumentService documentService)
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

        public PartialViewResult AdminEditor(Webpage page)
        {
            return PartialView("AdminEditor", page);
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