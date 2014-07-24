using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IArticleAdminService
    {
        IPagedList<Article> Search(ArticleSearchQuery query);
        List<SelectListItem> GetArticleSectionOptions();
        List<SelectListItem> GetPrimarySectionOptions();
    }
}