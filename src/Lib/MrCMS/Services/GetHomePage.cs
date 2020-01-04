using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using System.Linq;
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

        public Webpage Get()
        {
            return _cacheInHttpContext.GetForRequest("current.home-page", () => _repository
                .Readonly()
                .OrderBy(webpage => webpage.DisplayOrder)
                .FirstOrDefault(document => document.ParentId == null && document.Published));
        }
    }
}