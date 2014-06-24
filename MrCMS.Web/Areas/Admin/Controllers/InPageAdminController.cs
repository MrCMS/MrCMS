using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    [MrCMSACLRule(typeof (AdminBarACL), AdminBarACL.Show, ReturnEmptyResult = true)]
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
        [ValidateInput(false)]
        public JsonResult SaveBodyContent(UpdatePropertyData updatePropertyData)
        {
            return Json(_inPageAdminService.SaveBodyContent(updatePropertyData));
        }

        [ValidateInput(false)]
        public string GetUnformattedBodyContent(GetPropertyData getPropertyData)
        {
            return _inPageAdminService.GetUnformattedBodyContent(getPropertyData);
        }

        [ValidateInput(false)]
        public string GetFormattedBodyContent(GetPropertyData getPropertyData)
        {
            return _inPageAdminService.GetFormattedBodyContent(getPropertyData, this);
        }
    }
}