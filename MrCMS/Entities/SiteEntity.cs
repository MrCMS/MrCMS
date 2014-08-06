using MrCMS.Entities.Multisite;

namespace MrCMS.Entities
{
    public abstract class SiteEntity : SystemEntity
    {
        public virtual Site Site { get; set; }

        public virtual string SiteName
        {
            get { return Site != null ? Site.DisplayName : null; }
        }
    }
}