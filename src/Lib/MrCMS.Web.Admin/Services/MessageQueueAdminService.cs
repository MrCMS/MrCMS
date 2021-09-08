using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class MessageQueueAdminService : IMessageQueueAdminService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly ICurrentSiteLocator _siteLocator;

        public MessageQueueAdminService(ISession session, SiteSettings siteSettings, ICurrentSiteLocator siteLocator)
        {
            _session = session;
            _siteSettings = siteSettings;
            _siteLocator = siteLocator;
        }

        public async Task<IPagedList<QueuedMessage>> GetMessages(MessageQueueQuery searchQuery)
        {
            var site = _siteLocator.GetCurrentSite();
            var queryOver = _session.QueryOver<QueuedMessage>().Where(message => message.Site == site);
            if (searchQuery.From.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn <= searchQuery.To);
            if (!string.IsNullOrWhiteSpace(searchQuery.FromQuery))
                queryOver =
                    queryOver.Where(message =>
                        message.FromAddress.IsInsensitiveLike(searchQuery.FromQuery, MatchMode.Anywhere) ||
                        message.FromName.IsInsensitiveLike(searchQuery.FromQuery, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(searchQuery.ToQuery))
                queryOver =
                    queryOver.Where(message =>
                        message.ToAddress.IsInsensitiveLike(searchQuery.ToQuery, MatchMode.Anywhere) ||
                        message.ToName.IsInsensitiveLike(searchQuery.ToQuery, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(searchQuery.Subject))
                queryOver = queryOver.Where(message =>
                    message.Subject.IsInsensitiveLike(searchQuery.Subject, MatchMode.Anywhere));

            return await queryOver.OrderBy(message => message.CreatedOn).Desc
                .PagedAsync(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        public async Task<QueuedMessage> GetMessage(int id)
        {
            return await _session.GetAsync<QueuedMessage>(id);
        }
    }
}