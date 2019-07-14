using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    [Acl(typeof(AdminBarACL), AdminBarACL.Show, ReturnEmptyResult = true)]
    public class InPageAdminController : MrCMSAdminController
    {
        private readonly IInPageAdminService _inPageAdminService;

        public InPageAdminController(IInPageAdminService inPageAdminService)
        {
            _inPageAdminService = inPageAdminService;
        }

        public ActionResult InPageEditor(Webpage page)
        {
            return PartialView("InPageEditor", page);
        }

        [HttpPost]
        //[ValidateInput(false)]
        public JsonResult SaveContent(UpdatePropertyData updatePropertyData)
        {
            return Json(_inPageAdminService.SaveContent(updatePropertyData));
        }

        //[ValidateInput(false)]
        public PartialViewResult GetUnformattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(_inPageAdminService.GetContent(getPropertyData));
        }

        //[ValidateInput(false)]
        public PartialViewResult GetFormattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(_inPageAdminService.GetContent(getPropertyData));
        }
    }
}