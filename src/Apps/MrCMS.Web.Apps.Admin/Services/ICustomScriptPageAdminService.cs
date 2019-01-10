using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ICustomScriptPageAdminService
    {
        IPagedList<Webpage> Search(CustomScriptPagesSearchModel searchModel);
    }
}