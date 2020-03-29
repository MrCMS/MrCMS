using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IPageDefaultsAdminService
    {
        Task<List<PageDefaultsInfo>> GetAll();
        Task<List<SelectListItem>> GetUrlGeneratorOptions(Type type);
        Task<List<SelectListItem>> GetLayoutOptions();
        Task<DefaultsInfo> GetInfo(Type type);
        Task SetDefaults(DefaultsInfo info);

        Task EnableCache(string typeName);
        Task DisableCache(string typeName);
    }
}