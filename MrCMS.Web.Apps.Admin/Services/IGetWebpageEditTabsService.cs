using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetWebpageEditTabsService
    {
        List<AdminTabBase<Webpage>> GetEditTabs(IHtmlHelper html, Webpage page);
    }
}