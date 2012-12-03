using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Models
{
    public class AddWidgetModel
    {
        private readonly IEnumerable<SelectListItem> _types;
        private readonly int _layoutAreaId;
        private readonly string _returnUrl;
        private readonly string _name;
        private bool _isRecursive = true;

        public AddWidgetModel(IEnumerable<SelectListItem> types, int layoutAreaId, string returnUrl, string name)
        {
            _types = types;
            _layoutAreaId = layoutAreaId;
            _returnUrl = returnUrl;
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

        public string ReturnUrl
        {
            get { return _returnUrl; }
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