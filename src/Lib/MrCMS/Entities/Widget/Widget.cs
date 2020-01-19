using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Entities.Widget
{
    public abstract class Widget : SiteEntity
    {
        protected Widget()
        {
        }

        public virtual LayoutArea LayoutArea { get; set; }
        public int LayoutAreaId { get; set; }

        public virtual string Name { get; set; }

        [DisplayName("Custom Layout (leave blank to use default)")]
        public virtual string CustomLayout { get; set; }

        public virtual string WidgetClrType
        {
            get { return GetType().FullName; }
        }

        public virtual string WidgetTypeFormatted
        {
            get { return GetType().Name.BreakUpString(); }
        }

        public virtual Webpage Webpage { get; set; }
        public int? WebpageId { get; set; }
        public virtual int DisplayOrder { get; set; }


        [DisplayName("Cache output?")]
        public virtual bool Cache { get; set; }

        [DisplayName("Cache for how many seconds?")]
        public virtual int CacheLength { get; set; }

        [DisplayName("Cache expiry type")]
        public virtual CacheExpiryType CacheExpiryType { get; set; }

        [DefaultValue(true)]
        [DisplayName("Show on child pages")]
        public virtual bool IsRecursive { get; set; }

        //public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        //public virtual IList<HiddenWidget> HiddenOn { get; set; }
        //public virtual IList<ShownWidget> ShownOn { get; set; }

        public virtual bool HasProperties
        {
            get { return true; }
        }

    }
}