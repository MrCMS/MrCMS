using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class FormController : MrCMSController
    {
        private readonly IFormService _formService;

        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        public ActionResult Save(int id, FormCollection collection)
        {
            _formService.SaveFormData(id, collection);

            TempData["form-submitted"] = true;
            return Redirect(Referrer);
        }
    }
}