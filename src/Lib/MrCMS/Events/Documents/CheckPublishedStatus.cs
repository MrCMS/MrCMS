using System;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class CheckPublishedStatus : ICheckPublishedStatus
    {
        private readonly IGetDateTimeNow _getDateTimeNow;

        public CheckPublishedStatus(IGetDateTimeNow getDateTimeNow, IRepository<Webpage> repository)
        {
            _getDateTimeNow = getDateTimeNow;
        }
        public Task Check(Webpage webpage)
        {
            var now = _getDateTimeNow.LocalNow;

            if (webpage.PublishOn.HasValue && webpage.PublishOn <= now && webpage.Published == false)
            {
                webpage.Published = true;
            }

            return Task.CompletedTask;
        }
    }
}