using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public async Task<JsonResult> SaveContent(UpdatePropertyData updatePropertyData)
        {
            return Json(await _inPageAdminService.SaveContent(updatePropertyData));
        }

        //[ValidateInput(false)]
        public async Task<PartialViewResult> GetUnformattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(await _inPageAdminService.GetContent(getPropertyData));
        }

        //[ValidateInput(false)]
        public async Task<PartialViewResult> GetFormattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(await _inPageAdminService.GetContent(getPropertyData));
        }
    }
}