using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Models
{
    public class LayoutTreeListModel
    {
        private readonly List<SelectListItem> _items;
        private readonly List<SiteTree<Layout>> _siteTrees;

        public LayoutTreeListModel(List<SelectListItem> items, List<SiteTree<Layout>> siteTrees)
        {
            _items = items;
            _siteTrees = siteTrees;
        }

        public List<SelectListItem> Items
        {
            get { return _items; }
        }

        public List<SiteTree<Layout>> SiteTrees
        {
            get { return _siteTrees; }
        }
    }
}