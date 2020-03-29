using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class CustomScriptPageAdminService : ICustomScriptPageAdminService
    {
        private readonly IRepository<Webpage> _repository;

        public CustomScriptPageAdminService(IRepository<Webpage> repository)
        {
            _repository = repository;
        }


        public IPagedList<Webpage> Search(CustomScriptPagesSearchModel searchModel)
        {
            return _repository.Query()
                .Where(x =>
                    (x.CustomHeaderScripts != null && x.CustomHeaderScripts != "")
                    ||
                    (x.CustomFooterScripts != null && x.CustomFooterScripts != "")
                )
                .OrderBy(x => x.Name)
                .ToPagedList(searchModel.Page);
        }
    }
}