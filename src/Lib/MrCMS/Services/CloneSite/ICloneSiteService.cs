using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneSiteService
    {
        Task CloneData(Site site, List<SiteCopyOption> options);
    }
}