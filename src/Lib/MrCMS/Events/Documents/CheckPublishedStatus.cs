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

        public CheckPublishedStatus(IGetDateTimeNow getDateTimeNow)
        {
            _getDateTimeNow = getDateTimeNow;
        }
        public async Task Check(Webpage webpage)
        {
            var now = await _getDateTimeNow.GetLocalNow();

            if (webpage.PublishOn.HasValue && webpage.PublishOn <= now && webpage.Published == false)
            {
                webpage.Published = true; 
            }
        }
    }
}