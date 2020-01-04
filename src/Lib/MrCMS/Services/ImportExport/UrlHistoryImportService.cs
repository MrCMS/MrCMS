using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public class UrlHistoryImportService : IUrlHistoryImportService
    {
        private readonly IRepository<UrlHistory> _urlHistoryRepository;
        private readonly IRepository<Webpage> _webpageRepository;

        public UrlHistoryImportService(IRepository<UrlHistory> urlHistoryRepository, IRepository<Webpage> webpageRepository)
        {
            _urlHistoryRepository = urlHistoryRepository;
            _webpageRepository = webpageRepository;
        }

        public List<UrlHistoryInfo> GetAllOtherUrls(Webpage webpage)
        {
            var urlHistoryInfoList =
                _urlHistoryRepository.Readonly()
                    .Select(url => new UrlHistoryInfo
                    {
                        UrlSegment = url.UrlSegment,
                        WebpageId = url.WebpageId
                    }).ToList();
            var webpageHistoryInfoList =
                _webpageRepository.Readonly()
                    .Select(page => new UrlHistoryInfo
                    {
                        UrlSegment = page.UrlSegment,
                        WebpageId = page.Id
                    }).ToList();

            return urlHistoryInfoList.Union(webpageHistoryInfoList).Where(info => info.WebpageId != webpage.Id).ToList();
        }
    }
}