using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Caching;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Entities.Widget
{
    public abstract class Widget : SiteEntity
    {
        protected Widget()
        {
            ShownOn = new HashedSet<Webpage>();
            HiddenOn = new HashedSet<Webpage>();
        }
        public virtual LayoutArea LayoutArea { get; set; }

        public virtual string Name { get; set; }

        [DisplayName("Custom Layout (leave blank to use default)")]
        public virtual string CustomLayout { get; set; }

        public virtual string WidgetType { get { return GetType().Name; } }
        public virtual string WidgetTypeFormatted { get { return WidgetType.BreakUpString(); } }

        public virtual Webpage Webpage { get; set; }
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

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Webpage> HiddenOn { get; set; }
        public virtual Iesi.Collections.Generic.ISet<Webpage> ShownOn { get; set; }

        public virtual bool HasProperties { get { return true; } }
    }
}
