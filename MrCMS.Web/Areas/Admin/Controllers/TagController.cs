using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;
using NHibernate;

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