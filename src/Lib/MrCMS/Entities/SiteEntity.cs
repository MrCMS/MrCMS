using MrCMS.Entities.Multisite;

namespace MrCMS.Entities
{
    public abstract class SiteEntity : SystemEntity, IHaveSite
    {
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        public virtual string SiteName => Site?.DisplayName;
    }
}