using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageAdminSearchService : IWebpageAdminSearchService
    {
        private readonly IRepository<Webpage> _repository;

        public WebpageAdminSearchService(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public IPagedList<Webpage> Search(WebpageSearchQuery searchQuery)
        {
            var query = _repository.Query().Where(x => x.ParentId == searchQuery.ParentId);

            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{searchQuery.Query}%"));
            }

            return query.ToPagedList(searchQuery.Page);
        }
    }
}