using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ICloneSiteService
    {
        void CloneData(Site site, SiteCopyOptions options);
    }
}