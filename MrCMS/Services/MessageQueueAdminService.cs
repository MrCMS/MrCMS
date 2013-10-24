using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class MessageQueueAdminService : IMessageQueueAdminService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public MessageQueueAdminService(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery)
        {
            var queryOver = _session.QueryOver<QueuedMessage>();
            if (searchQuery.From.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn <= searchQuery.To);
            if (!string.IsNullOrWhiteSpace(searchQuery.FromQuery))
                queryOver =
                    queryOver.Where(
                        message =>
                        message.FromAddress.IsInsensitiveLike(searchQuery.FromQuery) ||
                        message.FromName.IsInsensitiveLike(searchQuery.FromQuery));
            if (!string.IsNullOrWhiteSpace(searchQuery.ToQuery))
                queryOver =
                    queryOver.Where(
                        message =>
                        message.ToAddress.IsInsensitiveLike(searchQuery.ToQuery) ||
                        message.ToName.IsInsensitiveLike(searchQuery.ToQuery));

            return queryOver.OrderBy(message => message.CreatedOn).Desc.Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }
    }
}