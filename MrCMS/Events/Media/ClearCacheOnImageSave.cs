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

        public void Execute(OnUpdatedArgs<MediaFile> args)
        {
            if (!args.Item.IsImage())
                return;

            if (!args.HasChanged(file => file.Title) && !args.HasChanged(file => file.Description))
                return;

            var tagPrefix = $"{MediaSettingExtensions.RenderTagPrefix}{GetUrlWithoutExtension(args.Item.FileUrl)}";
            var urlPrefix = $"{MediaSettingExtensions.RenderUrlPrefix}{GetUrlWithoutExtension(args.Item.FileUrl)}";

            _cacheManager.Clear(tagPrefix);
            _cacheManager.Clear(urlPrefix);
        }

        public string GetUrlWithoutExtension(string url)
        {
            return url.Substring(0, url.LastIndexOf("."));
        }
    }
}