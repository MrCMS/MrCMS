using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class FormController : MrCMSUIController
    {
        private readonly IDocumentService _documentService;
        private readonly IFormPostingHandler _formPostingHandler;

        public FormController(IDocumentService documentService, IFormPostingHandler formPostingHandler)
        {
            _documentService = documentService;
            _formPostingHandler = formPostingHandler;
        }

        [ValidateInput(false)]
        public ActionResult Save(int id)
        {
            var webpage = _documentService.GetDocument<Webpage>(id);
            var saveFormData = _formPostingHandler.SaveFormData(webpage, Request);

            TempData["form-submitted"] = true;
            TempData["form-submitted-message"] = saveFormData;
            // if any errors add form data to be renderered, otherwise form should be empty
            TempData["form-data"] = saveFormData.Any() ? Request.Form : null;
            return Redirect(Referrer.ToString());
        }
    }
}