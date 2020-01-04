using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneWebpageSiteParts
    {
        Task Clone(Webpage @from, Webpage to, SiteCloneContext siteCloneContext);
    }
}