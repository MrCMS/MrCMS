using System.Linq;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Filters;

namespace MrCMS.Website.Controllers
{
    public class FormController : MrCMSUIController
    {
        private readonly IFormPostingHandler _formPostingHandler;

        public FormController(IFormPostingHandler formPostingHandler)
        {
            _formPostingHandler = formPostingHandler;
        }

        [ValidateInput(false)]
        [GoogleRecaptcha]
        [DoNotCache]

        public ActionResult Save(int id)
        {
            var webpage = _formPostingHandler.GetWebpage(id);
            if (webpage?.IsDeleted != false)
                return new EmptyResult();
            var saveFormData = _formPostingHandler.SaveFormData(webpage, Request);

            TempData["form-submitted"] = true;
            TempData["form-submitted-message"] = saveFormData;
            // if any errors add form data to be renderered, otherwise form should be empty
            TempData["form-data"] = saveFormData.Any() ? Request.Form : null;

            var redirectUrl = Referrer.ToString();
            if (!string.IsNullOrEmpty(webpage.FormRedirectUrl) && !saveFormData.Any())
                redirectUrl = webpage.FormRedirectUrl;
            return Redirect(redirectUrl);
        }
    }
}