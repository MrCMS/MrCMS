using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Models
{
    public class AddPageWidgetModel
    {
        private readonly IEnumerable<SelectListItem> _types;
        private readonly int _layoutAreaId;
        private readonly int _pageId;
        private readonly string _name;
        private bool _isRecursive = true;

        public AddPageWidgetModel(IEnumerable<SelectListItem> types, int layoutAreaId, int pageId, string name)
        {
            _types = types;
            _layoutAreaId = layoutAreaId;
            _pageId = pageId;
            _name = name;
        }

        public IEnumerable<SelectListItem> Types
        {
            get { return _types; }
        }

        public int LayoutAreaId
        {
            get { return _layoutAreaId; }
        }

        public int PageId
        {
            get { return _pageId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsRecursive
        {
            get { return _isRecursive; }
        }
    }
}