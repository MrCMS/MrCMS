using System.Collections.Generic;
using System.Linq;
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

        public List<UrlHistoryInfo> GetAllOtherUrls(Webpage webpage)
        {
            var urlHistoryInfo = new UrlHistoryInfo();
            var urlHistoryInfoList =
                _session.QueryOver<UrlHistory>()
                    .SelectList(
                        builder =>
                            builder.Select(history => history.UrlSegment)
                                .WithAlias(() => urlHistoryInfo.UrlSegment)
                                .Select(history => history.Webpage.Id)
                                .WithAlias(() => urlHistoryInfo.WebpageId))
                    .TransformUsing(Transformers.AliasToBean<UrlHistoryInfo>())
                    .Cacheable()
                    .List<UrlHistoryInfo>();
            var webpageHistoryInfoList =
                _session.QueryOver<Webpage>()
                    .SelectList(
                        builder =>
                            builder.Select(page => page.UrlSegment)
                                .WithAlias(() => urlHistoryInfo.UrlSegment)
                                .Select(page => page.Id)
                                .WithAlias(() => urlHistoryInfo.WebpageId))
                    .TransformUsing(Transformers.AliasToBean<UrlHistoryInfo>())
                    .Cacheable()
                    .List<UrlHistoryInfo>();

            return urlHistoryInfoList.Union(webpageHistoryInfoList).Where(info => info.WebpageId != webpage.Id).ToList();
        }
    }
}