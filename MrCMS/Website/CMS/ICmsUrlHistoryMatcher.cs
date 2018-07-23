using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface ICmsUrlHistoryMatcher
    {
        Task<UrlHistoryLookupResult> TryMatch(string path);
    }
    public class CmsUrlHistoryMatcher : ICmsUrlHistoryMatcher
    {
        private readonly IRepository<UrlHistory> _repository;

        public CmsUrlHistoryMatcher(IRepository<UrlHistory> repository)
        {
            _repository = repository;
        }

        public async Task<UrlHistoryLookupResult> TryMatch(string path)
        {
            var history = _repository.Query()
                .FirstOrDefault(x => x.UrlSegment == path);

            var urlSegment = history?.Webpage?.UrlSegment;
            return string.IsNullOrWhiteSpace(urlSegment)
                ? new UrlHistoryLookupResult()
                : new UrlHistoryLookupResult { RedirectUrl = $"/{urlSegment}" };
        }
    }

}