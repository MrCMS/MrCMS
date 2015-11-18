using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MessageQueueAdminService : IMessageQueueAdminService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly Site _site;

        public MessageQueueAdminService(ISession session, SiteSettings siteSettings, Site site)
        {
            _session = session;
            _siteSettings = siteSettings;
            _site = site;
        }

        public IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery)
        {
            var queryOver = _session.QueryOver<QueuedMessage>().Where(message => message.Site == _site);
            if (searchQuery.From.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn <= searchQuery.To);
            if (!string.IsNullOrWhiteSpace(searchQuery.FromQuery))
                queryOver =
                    queryOver.Where(message => message.FromAddress.IsInsensitiveLike(searchQuery.FromQuery, MatchMode.Anywhere) || message.FromName.IsInsensitiveLike(searchQuery.FromQuery, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(searchQuery.ToQuery))
                queryOver =
                    queryOver.Where(message => message.ToAddress.IsInsensitiveLike(searchQuery.ToQuery, MatchMode.Anywhere) || message.ToName.IsInsensitiveLike(searchQuery.ToQuery, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(searchQuery.Subject))
                queryOver = queryOver.Where(message => message.Subject.IsInsensitiveLike(searchQuery.Subject, MatchMode.Anywhere));
            
            return queryOver.OrderBy(message => message.CreatedOn).Desc.Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        public QueuedMessage GetMessageBody(int id)
        {
            return _session.Get<QueuedMessage>(id);
        }
    }
}