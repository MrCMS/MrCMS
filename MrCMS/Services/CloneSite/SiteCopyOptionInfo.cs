using System.Reflection;

namespace MrCMS.Services.CloneSite
{
    internal class SiteCopyOptionInfo
    {
        public SiteCopyOptionInfo(int siteId, ICloneSiteParts cloneSiteParts)
        {
            CloneSiteParts = cloneSiteParts;
            SiteId = siteId;
        }

        internal int SiteId { get; private set; }
        internal ICloneSiteParts CloneSiteParts { get; private set; }

        internal int Order
        {
            get
            {
                return CloneSiteExtensions.GetCloneSitePartOrder(CloneSiteParts.GetType());
            }
        }
    }
}