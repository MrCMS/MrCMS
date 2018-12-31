using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Services.CloneSite
{
    public interface ICloneSiteService
    {
        void CloneData(Site site, List<SiteCopyOption> options);
    }
}