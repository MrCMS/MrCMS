using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TagController : AdminController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public JsonResult Search(string term, int documentId)
        {
            IEnumerable<AutoCompleteResult> result = _tagService.Search(term, documentId);

            return Json(result);
        }
    }
}