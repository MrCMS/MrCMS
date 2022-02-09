using System.ComponentModel;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Entities.Widget
{
    public abstract class Widget : SiteEntity
    {

        public virtual LayoutArea LayoutArea { get; set; }

        public virtual string Name { get; set; }

        [DisplayName("Custom Layout (leave blank to use default)")]
        public virtual string CustomLayout { get; set; }

        public virtual string WidgetType => GetType().FullName;

        public virtual string WidgetTypeFormatted => GetType().Name.BreakUpString();

        public virtual int DisplayOrder { get; set; }


        [DisplayName("Cache output?")]
        public virtual bool Cache { get; set; }

        [DisplayName("Cache for how many seconds?")]
        public virtual int CacheLength { get; set; }

        [DisplayName("Cache expiry type")]
        public virtual CacheExpiryType CacheExpiryType { get; set; }

        public virtual bool HasProperties => true;
    }
}