using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class FormController : MrCMSUIController
    {
        private readonly IDocumentService _documentService;
        private readonly IFormService _formService;

        public FormController(IDocumentService documentService, IFormService formService)
        {
            _documentService = documentService;
            _formService = formService;
        }

        public ActionResult Save(int id)
        {
            var webpage = _documentService.GetDocument<Webpage>(id);
            var saveFormData = _formService.SaveFormData(webpage, Request);
            
            TempData["form-submitted"] = true;
            TempData["form-submitted-message"] = saveFormData;
            // if any errors add form data to be renderered, otherwise form should be empty
            TempData["form-data"] = saveFormData.Any() ? Request.Form : null;
            return Redirect(Referrer.ToString());
        }
    }
}