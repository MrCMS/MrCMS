using System;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class UniversalSearchItemQuickSearch
    {
        private readonly UniversalSearchItem _item;

        public UniversalSearchItemQuickSearch(UniversalSearchItem item)
        {
            _item = item;
        }

        public string id
        {
            get { return _item.SearchGuid.ToString(); }
        }

        public string value
        {
            get { return _item.DisplayName; }
        }

        public string actionUrl
        {
            get { return _item.ActionUrl; }
        }

        public string systemType
        {
            get { return _item.SystemType; }
        }

        public string displayType
        {
            get
            {
                Type typeByName = TypeHelper.GetTypeByName(_item.SystemType);
                return typeByName == null ? "" : typeByName.Name.BreakUpString();
            }
        }

        public string systemId
        {
            get { return _item.Id.ToString(); }
        }
    }
}