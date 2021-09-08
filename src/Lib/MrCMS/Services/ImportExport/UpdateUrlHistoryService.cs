using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport
{
    public class UpdateUrlHistoryService : IUpdateUrlHistoryService
    {
        private readonly IRepository<UrlHistory> _urlHistoryRepository;

        public UpdateUrlHistoryService(IRepository<UrlHistory> urlHistoryRepository)
        {
            _urlHistoryRepository = urlHistoryRepository;
        }

        public async Task SetUrlHistory(DocumentImportDTO documentDto, Webpage webpage)
        {
            List<string> urlsToAdd =
                documentDto.UrlHistory.Where(
                    s =>
                        !webpage.Urls.Select(history => history.UrlSegment)
                            .Contains(s, StringComparer.InvariantCultureIgnoreCase)).ToList();
            List<UrlHistory> urlsToRemove =
                webpage.Urls.Where(
                        history =>
                            !documentDto.UrlHistory.Contains(history.UrlSegment,
                                StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            if (!urlsToAdd.Any() && !urlsToRemove.Any())
                return;
            await UpdateUrlHistories(webpage, urlsToAdd, urlsToRemove);
        }

        private async Task UpdateUrlHistories(Webpage webpage, List<string> urlsToAdd, List<UrlHistory> urlsToRemove)
        {
            await _urlHistoryRepository.TransactAsync(async repo =>
            {
                await AddUrls(repo, webpage, urlsToAdd);

                await RemoveUrls(repo, webpage, urlsToRemove);
            });
        }

        private static async Task RemoveUrls(IRepository<UrlHistory> repository, Webpage webpage,
            List<UrlHistory> urlsToRemove)
        {
            foreach (UrlHistory history in urlsToRemove)
            {
                webpage.Urls.Remove(history);
                history.Webpage = null;
                await repository.Update(history);
            }
        }

        private static async Task AddUrls(IRepository<UrlHistory> repository, Webpage webpage, List<string> urlsToAdd)
        {
            foreach (string item in urlsToAdd)
            {
                UrlHistory history =
                    await repository.Query().FirstOrDefaultAsync(urlHistory => urlHistory.UrlSegment == item);
                bool isNew = history == null;
                if (isNew)
                {
                    history = new UrlHistory {UrlSegment = item, Webpage = webpage};
                    await repository.Add(history);
                }
                else
                    history.Webpage = webpage;

                if (!webpage.Urls.Contains(history))
                    webpage.Urls.Add(history);
                await repository.Update(history);
            }
        }
    }
}