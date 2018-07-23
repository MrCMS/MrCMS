using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsUnpublished : IOnUpdating<Webpage>
    {
        public void Execute(OnUpdatingArgs<Webpage> args)
        {
            var now = DateTime.UtcNow;
            var webpage = args.Item;
            if (webpage.Published && (webpage.PublishOn == null || webpage.PublishOn.Value.ToUniversalTime() > now))
            {
                webpage.Published = false;
            }
        }
    }
}