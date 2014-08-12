using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IPageDefaultsAdminService
    {
        List<PageDefaultsInfo> GetAll();
        List<SelectListItem> GetUrlGeneratorOptions(Type type);
        List<SelectListItem> GetLayoutOptions();
        DefaultsInfo GetInfo(Type type);
        void SetDefaults(DefaultsInfo info);
    }
}