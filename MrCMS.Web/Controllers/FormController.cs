using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class FormController : MrCMSUIController
    {
        private readonly IFormService _formService;

        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        public ActionResult Save(Webpage webpage)
        {
            var saveFormData = _formService.SaveFormData(webpage, Request);

            TempData["form-submitted"] = true;
            TempData["form-submitted-message"] = saveFormData;
            // if any errors add form data to be renderered, otherwise form should be empty
            TempData["form-data"] = saveFormData.Any() ? Request.Form : null;
            return Redirect(Referrer);
        }
    }
}