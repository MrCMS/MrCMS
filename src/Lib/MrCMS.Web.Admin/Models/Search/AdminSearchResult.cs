using MrCMS.Entities;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.TextSearch.Entities;

namespace MrCMS.Web.Admin.Models.Search
{
    public class AdminSearchResult
    {
        private readonly TextSearchItem _item;
        private readonly SystemEntity _entity;

        public AdminSearchResult(TextSearchItem item, SystemEntity entity)
        {
            _item = item;
            _entity = entity;
        }

        public string ActionUrl => $"/Admin/{_item.EntityType}/{action}/{_item.EntityId}";
        private string action => _entity is MediaCategory ? "Show" : "Edit";

        public string DisplayName
        {
            get { return _item.DisplayName; }
        }

        public SystemEntity Entity
        {
            get { return _entity; }
        }

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