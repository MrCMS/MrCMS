using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsPublished : IOnUpdating<Webpage>, IOnAdding<Webpage>
    {
        private readonly IGetNowForSite _getNowForSite;

        public MarkWebpageAsPublished(IGetNowForSite getNowForSite)
        {
            _getNowForSite = getNowForSite;
        }

        public void Execute(OnUpdatingArgs<Webpage> args)
        {
            var now = _getNowForSite.Now;
            var webpage = args.Item;
            if (webpage.PublishOn.HasValue && webpage.PublishOn <= now && webpage.Published == false)
            {
                webpage.Published = true;
            }
        }

        public void Execute(OnAddingArgs<Webpage> args)
        {
            var now = _getNowForSite.Now;
            var webpage = args.Item;
            if (webpage.PublishOn.HasValue && webpage.PublishOn <= now && webpage.Published == false)
            {
                webpage.Published = true;
            }
        }
    }
}