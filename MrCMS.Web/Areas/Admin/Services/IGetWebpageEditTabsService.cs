using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetWebpageEditTabsService
    {
        List<WebpageTabBase> GetEditTabs(Webpage page);
    }
}