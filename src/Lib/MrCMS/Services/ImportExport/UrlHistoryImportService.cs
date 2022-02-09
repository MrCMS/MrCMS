using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Services.ImportExport
{
    public class UrlHistoryImportService : IUrlHistoryImportService
    {
        private readonly ISession _session;

        public UrlHistoryImportService(ISession session)
        {
            _session = session;
        }

        public async Task<List<UrlHistoryInfo>> GetAllOtherUrls(Webpage webpage)
        {
            var urlHistoryInfo = new UrlHistoryInfo();
            var urlHistoryInfoList =
                await _session.QueryOver<UrlHistory>()
                    .Where(x => x.Webpage != null)
                    .SelectList(
                        builder =>
                            builder.Select(history => history.UrlSegment)
                                .WithAlias(() => urlHistoryInfo.UrlSegment)
                                .Select(history => history.Webpage.Id)
                                .WithAlias(() => urlHistoryInfo.WebpageId))
                    .TransformUsing(Transformers.AliasToBean<UrlHistoryInfo>())
                    .Cacheable()
                    .ListAsync<UrlHistoryInfo>();
            var webpageHistoryInfoList =
                await _session.QueryOver<Webpage>()
                    .SelectList(
                        builder =>
                            builder.Select(page => page.UrlSegment)
                                .WithAlias(() => urlHistoryInfo.UrlSegment)
                                .Select(page => page.Id)
                                .WithAlias(() => urlHistoryInfo.WebpageId))
                    .TransformUsing(Transformers.AliasToBean<UrlHistoryInfo>())
                    .Cacheable()
                    .ListAsync<UrlHistoryInfo>();

            return urlHistoryInfoList.Union(webpageHistoryInfoList).Where(info => info.WebpageId != webpage.Id)
                .ToList();
        }
    }
}