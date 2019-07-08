using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsUnpublished : IOnUpdating<Webpage>
    {
        private readonly IGetDateTimeNow _getDateTimeNow;

        public MarkWebpageAsUnpublished(IGetDateTimeNow getDateTimeNow)
        {
            _getDateTimeNow = getDateTimeNow;
        }

        public void Execute(OnUpdatingArgs<Webpage> args)
        {
            var now = _getDateTimeNow.UtcNow;
            var webpage = args.Item;
            if (webpage.Published && (webpage.PublishOn == null || webpage.PublishOn.Value.ToUniversalTime() > now))
            {
                webpage.Published = false;
            }
        }
    }
}