using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events
{
    public class AddUrlHistory : OnDataUpdated<Webpage>
    {
        private readonly IRepository<UrlHistory> _repository;

        public AddUrlHistory(IRepository<UrlHistory> repository)
        {
            _repository = repository;
        }

        public override async Task Execute(ChangeInfo data)
        {
            if (!(data.Entity() is Webpage webpage))
                return;
            var updated = data.PropertiesUpdated.Any(x => x.Name == nameof(Webpage.UrlSegment));
            if (!updated)
                return;
            var propInfo = data.PropertiesUpdated.First(x => x.Name == nameof(Webpage.UrlSegment));
            await SaveChangedUrl(propInfo.OriginalValue.ToString(), propInfo.CurrentValue.ToString(), webpage.Id);
        }

        private async Task SaveChangedUrl(string oldUrl, string newUrl, int webpageId)
        {
            //check that the URL is different and doesn't already exist in the URL history table.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oldUrl, newUrl) && !await CheckUrlExistence(oldUrl))
            {
                var createdOn = DateTime.UtcNow;
                var urlHistory = new UrlHistory
                {
                    WebpageId = webpageId,
                    UrlSegment = oldUrl,
                    CreatedOn = createdOn,
                    UpdatedOn = createdOn
                };
                await _repository.Add(urlHistory);
            }
        }

        private Task<bool> CheckUrlExistence(string url)
        {
            return _repository.Readonly().AnyAsync(history => history.UrlSegment == url);
        }
    }
}