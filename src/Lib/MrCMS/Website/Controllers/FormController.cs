using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
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

        [HttpPost]
        [GoogleRecaptcha]
        [Route("save-form/{id}")]
        public ActionResult Save(int id)
        {
            var form = _formPostingHandler.GetForm(id);
            if (form?.IsDeleted != false)
                return new EmptyResult();
            var saveFormData = _formPostingHandler.SaveFormData(form, Request);
            
            TempData["form-submitted"] = true;
            TempData.Set(saveFormData, "form-submitted-message");
            // if any errors add form data to be renderered, otherwise form should be empty
            TempData.Set(saveFormData.Any() ? Request.Form : null, "form-data");

            var redirectUrl = _formPostingHandler.GetRefererUrl();
            if (!saveFormData.Any() && !string.IsNullOrEmpty(form.FormRedirectUrl))
                redirectUrl = form.FormRedirectUrl;
            if (!string.IsNullOrWhiteSpace(redirectUrl))
                return Redirect(redirectUrl);
            return Redirect("~/");
        }
    }
}