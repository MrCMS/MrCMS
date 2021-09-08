using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IPageDefaultsAdminService
    {
        Task<List<PageDefaultsInfo>> GetAll();
        List<SelectListItem> GetUrlGeneratorOptions(string typeName);
        Task<List<SelectListItem>> GetLayoutOptions();
        DefaultsInfo GetInfo(string typeName);
        Task SetDefaults(DefaultsInfo info);

        Task EnableCache(string typeName);
        Task DisableCache(string typeName);
    }
}