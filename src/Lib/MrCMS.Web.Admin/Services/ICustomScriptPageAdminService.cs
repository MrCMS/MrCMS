using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface ICustomScriptPageAdminService
    {
        IPagedList<Webpage> Search(CustomScriptPagesSearchModel searchModel);
    }
}