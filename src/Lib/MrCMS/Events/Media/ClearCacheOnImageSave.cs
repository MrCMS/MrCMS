using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Media
{
    public class ClearCacheOnImageSave : OnDataUpdated<MediaFile>
    {
        private readonly ICacheManager _cacheManager;

        public ClearCacheOnImageSave(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public string GetUrlWithoutExtension(string url)
        {
            return url.Substring(0, url.LastIndexOf("."));
        }

        public override Task Execute(ChangeInfo data)
        {
            if (!MediaFileExtensions.IsImageExtension(data.Properties.GetValue<string>(nameof(MediaFile.FileExtension))))
                return Task.CompletedTask;

            if (!data.PropertiesUpdated.Any(x=>x.Name == nameof(MediaFile.Title) || x.Name == nameof(MediaFile.Description) ))
                return Task.CompletedTask;

            var fileUrl = data.Properties.GetValue<string>(nameof(MediaFile.FileUrl));
            var tagPrefix = $"{MediaSettingExtensions.RenderTagPrefix}{GetUrlWithoutExtension(fileUrl)}";
            var urlPrefix = $"{MediaSettingExtensions.RenderUrlPrefix}{GetUrlWithoutExtension(fileUrl)}";

            _cacheManager.Clear(tagPrefix);
            _cacheManager.Clear(urlPrefix);

            return Task.CompletedTask;
        }
    }
}