using MrCMS.Entities.Multisite;

namespace MrCMS.Entities
{
    public interface IHaveSite
    {
        int SiteId { get; set; }
        Site Site { get; set; }
    }
}