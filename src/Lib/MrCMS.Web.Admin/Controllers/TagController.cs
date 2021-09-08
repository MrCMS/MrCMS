using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class TagController : MrCMSAdminController
    {
        private readonly ITagAdminService _tagAdminService;

        public TagController(ITagAdminService tagAdminService)
        {
            _tagAdminService = tagAdminService;
        }

        public async Task<JsonResult> Search(string term)
        {
            IEnumerable<AutoCompleteResult> result = await _tagAdminService.Search(term);

            return Json(result);
        }
    }
}