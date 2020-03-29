using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ICustomScriptPageAdminService
    {
        IPagedList<Webpage> Search(CustomScriptPagesSearchModel searchModel);
    }
}