using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Web.Areas.Admin.Models.Search
{
    public class AdminSearchResult
    {
        private readonly UniversalSearchItem _item;
        private readonly SystemEntity _entity;

        public AdminSearchResult(UniversalSearchItem item, SystemEntity entity)
        {
            _item = item;
            _entity = entity;
        }

        public string ActionUrl { get { return _item.ActionUrl; } }
        public string DisplayName { get { return _item.DisplayName; } }
        public SystemEntity Entity { get { return _entity; } }
        public string Type
        {
            get
            {
                var typeByName = TypeHelper.GetTypeByName(_item.SystemType);
                return typeByName == null ? "" : typeByName.Name.BreakUpString(); 
            }
        }
    }
}