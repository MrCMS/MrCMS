using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport
{
    public class UpdateUrlHistoryService : IUpdateUrlHistoryService
    {
        private readonly ISession _session;

        public UpdateUrlHistoryService(ISession session)
        {
            _session = session;
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
            foreach (string item in urlsToAdd)
            {
                UrlHistory history =
                    _session.Query<UrlHistory>().FirstOrDefault(urlHistory => urlHistory.UrlSegment == item);
                bool isNew = history == null;
                if (isNew)
                {
                    history = new UrlHistory {UrlSegment = item, Webpage = webpage};
                    _session.Transact(session => session.Save(history));
                }
                else
                    history.Webpage = webpage;
                if (!webpage.Urls.Contains(history))
                    webpage.Urls.Add(history);
                _session.Transact(session => session.Update(history));
            }

            foreach (UrlHistory history in urlsToRemove)
            {
                webpage.Urls.Remove(history);
                history.Webpage = null;
                UrlHistory closureHistory = history;
                _session.Transact(session => session.Update(closureHistory));
            }
        }
    }
}