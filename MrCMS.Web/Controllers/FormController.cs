using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class FormController : MrCMSController
    {
        private readonly IDocumentService _documentService;

        public FormController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public ActionResult Save(int id, FormCollection collection)
        {
            _documentService.SaveFormData(id, collection);

            TempData["form-submitted"] = true;
            return Redirect(Referrer);
        }
    }
}