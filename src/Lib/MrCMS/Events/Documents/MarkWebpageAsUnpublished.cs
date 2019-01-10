using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsUnpublished : IOnUpdating<Webpage>
    {
        private readonly IGetNowForSite _getNowForSite;

        public MarkWebpageAsUnpublished(IGetNowForSite getNowForSite)
        {
            _getNowForSite = getNowForSite;
        }

        public void Execute(OnUpdatingArgs<Webpage> args)
        {
            var now = _getNowForSite.Now;
            var webpage = args.Item;
            if (webpage.Published && (webpage.PublishOn == null || webpage.PublishOn.Value > now))
            {
                webpage.Published = false;
            }
        }
    }
}