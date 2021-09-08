using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Media
{
    public class ClearCacheOnImageSave : IOnUpdated<MediaFile>
    {
        private readonly ICacheManager _cacheManager;

        public ClearCacheOnImageSave(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Execute(OnUpdatedArgs<MediaFile> args)
        {
            if (!args.Item.IsImage())
                return Task.CompletedTask;

            if (!args.HasChanged(file => file.Title) && !args.HasChanged(file => file.Description))
                return Task.CompletedTask;

            _cacheManager.Clear();
            return Task.CompletedTask;
        }

        public string GetUrlWithoutExtension(string url)
        {
            return url.Substring(0, url.LastIndexOf("."));
        }
    }
}