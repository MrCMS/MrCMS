using System.Collections.Generic;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.Models
{
    public class ArticleArchiveModel
    {
        public ArticleList ArticleList { get; set; }
        public ArticleArchive ArticleArchive { get; set; }
        public IList<ArchiveModel> ArticleYearsAndMonths { get; set; }
        public string Year { get { return CurrentRequestData.CurrentContext.Request["year"]; } }
        public string Month { get { return CurrentRequestData.CurrentContext.Request["year"]; } }
    }
}