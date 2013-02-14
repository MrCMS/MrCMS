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

        public ActionResult Save(Webpage webpage, FormCollection collection)
        {
            _formService.SaveFormData(webpage, collection);

            TempData["form-submitted"] = true;
            return Redirect(Referrer);
        }
    }
}