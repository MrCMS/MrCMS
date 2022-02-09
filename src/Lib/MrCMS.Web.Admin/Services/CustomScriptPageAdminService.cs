using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class CustomScriptPageAdminService : ICustomScriptPageAdminService
    {
        private readonly ISession _session;

        public CustomScriptPageAdminService(ISession session)
        {
            _session = session;
        }

        public async Task<IPagedList<Webpage>> Search(CustomScriptPagesSearchModel searchModel)
        {
            return await _session.QueryOver<Webpage>()
                .Where(x =>
                    (x.CustomHeaderScripts != null && x.CustomHeaderScripts != "")
                    ||
                    (x.CustomFooterScripts != null && x.CustomFooterScripts != "")
                )
                .OrderBy(x => x.Name).Asc
                .PagedAsync(searchModel.Page);
        }
    }
}