using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.AddOns.Pages.Portfolio;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class TeaserPortfolio : Widget
    {
        public virtual PortfolioItem PortfolioItem { get; set; }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            var documentService = new DocumentService(session, new SiteSettings());

            viewData["portfolioItems"] = GetSelectListItems(session.QueryOver<PortfolioItem>().List());
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<PortfolioItem> pages)
        {
            return pages.Select(page => new SelectListItem {Selected = false, Text = page.Name, Value = page.Id.ToString()}).ToList();
        }
    }
}