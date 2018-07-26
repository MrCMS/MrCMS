using MrCMS.Entities.Multisite;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneSiteParts
    {
        void Clone(Site @from, Site to, SiteCloneContext siteCloneContext);
    }
}