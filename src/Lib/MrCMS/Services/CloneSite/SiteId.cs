using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Services.CloneSite
{
    internal class SiteId : IGetSiteId
    {
        private readonly int _id;

        private SiteId(int id)
        {
            _id = id;
        }
        public int GetId()
        {
            return _id;
        }

        internal static SiteId GetForId(int id) => new SiteId(id);
        internal static SiteId GetForSite(Site site) => new SiteId(site?.Id ?? -1);
    }
}