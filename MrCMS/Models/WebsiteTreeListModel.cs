using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Models
{
    public class WebsiteTreeListModel
    {
        private readonly List<SelectListItem> _items;
        private readonly List<SiteTree<Webpage>> _siteTrees;

        public WebsiteTreeListModel(List<SelectListItem> items, List<SiteTree<Webpage>> siteTrees)
        {
            _items = items;
            _siteTrees = siteTrees;
        }

        public List<SelectListItem> Items
        {
            get { return _items; }
        }

        public List<SiteTree<Webpage>> SiteTrees
        {
            get { return _siteTrees; }
        }
    }
}