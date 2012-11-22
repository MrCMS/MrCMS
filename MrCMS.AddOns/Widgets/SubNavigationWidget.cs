using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using System.Linq;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class SubNavigationWidget : Widget
    {
        public virtual Webpage Page { get; set; }

        public override object GetModel(ISession session)
        {
            if (Page == null)
                return null;

            var webpages = Page.Children.OfType<Webpage>().OrderBy(document => document.DisplayOrder).Select(
                webpage => new NavigationRecord
                               {
                                   Text = webpage.Name,
                                   Url = "/" + webpage.UrlSegment
                               }).ToList();

            return new NavigationList(webpages);
        }

        public override void SetDropdownData(ViewDataDictionary viewData, ISession session)
        {
            var documentService = new DocumentService(session, new SiteSettings());
            var navigationService = new NavigationService(documentService, null);

            viewData["Pages"] = GetSelectListItems(navigationService.GetWebsiteTree().Children, 0);
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<SiteTreeNode<Webpage>> nodes, int depth)
        {
            var items = new List<SelectListItem>();

            foreach (var node in nodes)
            {
                items.Add(new SelectListItem { Selected = false, Text = GetDashes(depth) + node.Name, Value = node.Id.ToString() });

                if (node.Children.Any())
                    items.AddRange(GetSelectListItems(node.Children, depth + 1));
            }

            return items;
        }

        private string GetDashes(int depth)
        {
            return string.Empty.PadRight(depth * 2, '-');
        }
    }
}