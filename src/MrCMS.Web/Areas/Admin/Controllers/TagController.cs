using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TagController : MrCMSAdminController
    {
        private readonly ITagAdminService _tagAdminService;

        public TagController(ITagAdminService tagAdminService)
        {
            _tagAdminService = tagAdminService;
        }

        public JsonResult Search(string term)
        {
            IEnumerable<AutoCompleteResult> result = _tagAdminService.Search(term);

            return Json(result);
        }
    }
}