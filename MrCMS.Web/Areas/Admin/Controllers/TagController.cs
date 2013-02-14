using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TagController : MrCMSAdminController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public JsonResult Search(Document document, string term)
        {
            IEnumerable<AutoCompleteResult> result = _tagService.Search(document, term);

            return Json(result);
        }
    }
}