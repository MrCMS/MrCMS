using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
{
    [Acl(typeof(AdminBarACL), AdminBarACL.Show, ReturnEmptyResult = true)]
    public class InPageAdminController : MrCMSAdminController
    {
        private readonly IInPageAdminService _inPageAdminService;
        private readonly IWebpageAdminService _webpageUiService;

        public InPageAdminController(IInPageAdminService inPageAdminService, IWebpageAdminService webpageUiService)
        {
            _inPageAdminService = inPageAdminService;
            _webpageUiService = webpageUiService;
        }

        public async Task<ActionResult> InPageEditor(int id)
        {
            return PartialView("InPageEditor", await _webpageUiService.GetWebpage(id));
        }

        [HttpPost]
        public async Task<JsonResult> SaveContent([FromBody] UpdatePropertyData updatePropertyData)
        {
            return Json(await _inPageAdminService.SaveContent(updatePropertyData));
        }

        public async Task<PartialViewResult> GetUnformattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(await _inPageAdminService.GetContent(getPropertyData));
        }

        public async Task<PartialViewResult> GetFormattedContent(GetPropertyData getPropertyData)
        {
            return PartialView(await _inPageAdminService.GetContent(getPropertyData));
        }
    }
}