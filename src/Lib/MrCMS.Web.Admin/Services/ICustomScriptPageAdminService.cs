using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface ICustomScriptPageAdminService
    {
        Task<IPagedList<Webpage>> Search(CustomScriptPagesSearchModel searchModel);
    }
}