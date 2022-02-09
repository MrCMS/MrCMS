using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneSiteParts
    {
        Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext);
    }
}