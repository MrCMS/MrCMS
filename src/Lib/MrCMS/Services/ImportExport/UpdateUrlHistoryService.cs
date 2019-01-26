using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate;
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

        public void SetUrlHistory(DocumentImportDTO documentDto, Webpage webpage)
        {
            List<string> urlsToAdd =
                documentDto.UrlHistory.Where(
                    s =>
                        !webpage.Urls.Select(history => history.UrlSegment)
                            .Contains(s, StringComparer.InvariantCultureIgnoreCase)).ToList();
            List<UrlHistory> urlsToRemove =
                webpage.Urls.Where(
                    history =>
                        !documentDto.UrlHistory.Contains(history.UrlSegment, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            if (!urlsToAdd.Any() && !urlsToRemove.Any())
                return;
            UpdateUrlHistories(webpage, urlsToAdd, urlsToRemove);
        }

        private void UpdateUrlHistories(Webpage webpage, List<string> urlsToAdd, List<UrlHistory> urlsToRemove)
        {
            _urlHistoryRepository.Transact(session =>
            {
                AddUrls(webpage, urlsToAdd);

                RemoveUrls(webpage, urlsToRemove);
            });
        }

        private void RemoveUrls(Webpage webpage, List<UrlHistory> urlsToRemove)
        {
            foreach (UrlHistory history in urlsToRemove)
            {
                webpage.Urls.Remove(history);
                history.Webpage = null;
                _urlHistoryRepository.Update(history);
            }
        }

        private void AddUrls(Webpage webpage, List<string> urlsToAdd)
        {
            foreach (string item in urlsToAdd)
            {
                UrlHistory history =
                    _urlHistoryRepository.Query().FirstOrDefault(urlHistory => urlHistory.UrlSegment == item);
                bool isNew = history == null;
                if (isNew)
                {
                    history = new UrlHistory { UrlSegment = item, Webpage = webpage };
                    _urlHistoryRepository.Add(history);
                }
                else
                    history.Webpage = webpage;
                if (!webpage.Urls.Contains(history))
                    webpage.Urls.Add(history);
                _urlHistoryRepository.Update(history);
            }
        }
    }
}