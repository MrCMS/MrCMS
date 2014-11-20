using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneWebpageSiteParts
    {
        void Clone(Webpage @from, Webpage to, SiteCloneContext siteCloneContext);
    }
}