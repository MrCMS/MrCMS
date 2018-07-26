using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetWebpageEditTabsService
    {
        List<WebpageTabBase> GetEditTabs(Webpage page);
    }
}