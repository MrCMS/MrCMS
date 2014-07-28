using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUrlGeneratorSettingsAdminService
    {
        List<UrlGeneratorSettingInfo> GetAll();
        List<SelectListItem> GetUrlGeneratorOptions(Type type);
        DefaultGeneratorInfo GetInfo(Type type);
        void SetDefault(DefaultGeneratorInfo info);
    }
}