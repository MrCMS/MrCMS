using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Settings;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class MessageQueueAdminService : IMessageQueueAdminService
    {
        private readonly IRepository<QueuedMessage> _repository;
        private readonly SiteSettings _siteSettings;

        public MessageQueueAdminService(IRepository<QueuedMessage> repository, SiteSettings siteSettings)
        {
            _repository = repository;
            _siteSettings = siteSettings;
        }

        public IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery)
        {
            var queryOver = _repository.Readonly();
            if (searchQuery.From.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn <= searchQuery.To);
            if (!string.IsNullOrWhiteSpace(searchQuery.FromQuery))
                queryOver =
                    queryOver.Where(message =>
                        EF.Functions.Like(message.FromAddress, $"%{searchQuery.FromQuery}%") ||
                        EF.Functions.Like(message.FromName, $"%{searchQuery.FromQuery}%"));
            if (!string.IsNullOrWhiteSpace(searchQuery.ToQuery))
                queryOver =
                    queryOver.Where(message =>
                        EF.Functions.Like(message.ToAddress, $"%{searchQuery.ToQuery}%") ||
                        EF.Functions.Like(message.ToName, $"%{searchQuery.ToQuery}%"));

            if (!string.IsNullOrWhiteSpace(searchQuery.Subject))
                queryOver = queryOver.Where(message => EF.Functions.Like(message.Subject, $"%{searchQuery.Subject}%"));

            return queryOver.OrderByDescending(message => message.CreatedOn).ToPagedList(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        public QueuedMessage GetMessageBody(int id)
        {
            return _repository.GetDataSync(id);
        }
    }
}