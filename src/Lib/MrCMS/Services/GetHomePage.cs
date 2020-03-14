using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;

namespace MrCMS.Services
{
    public class GetHomePage : IGetHomePage
    {
        private readonly IRepository<Webpage> _repository;
        private readonly ICacheInHttpContext _cacheInHttpContext;

        public GetHomePage(IRepository<Webpage> repository, ICacheInHttpContext cacheInHttpContext)
        {
            _repository = repository;
            _cacheInHttpContext = cacheInHttpContext;
        }

        public async Task<Webpage> Get()
        {
            //return await _cacheInHttpContext.GetForRequest("current.home-page", () => _repository
            //    .Query()
            //    .OrderBy(webpage => webpage.DisplayOrder)
            //    .FirstOrDefaultAsync(document => document.ParentId == null && document.Published));
            return await _repository
                .Query()
                .OrderBy(webpage => webpage.DisplayOrder)
                .FirstOrDefaultAsync(document => document.ParentId == null && document.Published);
        }
    }
}