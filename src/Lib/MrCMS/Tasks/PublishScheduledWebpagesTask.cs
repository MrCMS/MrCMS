using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class PublishScheduledWebpagesTask : SchedulableTask
    {
        private readonly IGlobalRepository<Webpage> _repository;
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly INotificationDisabler _notificationDisabler;

        public PublishScheduledWebpagesTask(IGlobalRepository<Webpage> repository, IGetDateTimeNow getDateTimeNow, INotificationDisabler notificationDisabler)
        {
            _repository = repository;
            _getDateTimeNow = getDateTimeNow;
            _notificationDisabler = notificationDisabler;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override async Task OnExecute(CancellationToken token)
        {
            using (_notificationDisabler.Disable())
            {
                var now = await _getDateTimeNow.GetLocalNow();
                var due = await _repository.Query().Where(x => !x.Published && x.PublishOn <= now).ToListAsync(token);
                if (!due.Any())
                    return;
                foreach (var webpage in due)
                {
                    webpage.Published = true;
                }

                await _repository.UpdateRange(due, token);
            }
        }
    }
}