using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetCurrentLayout : IGetCurrentLayout
    {
        private readonly PageDefaultsSettings _pageDefaultsSettings;
        private readonly IRepository<Layout> _repository;
        private readonly SiteSettings _siteSettings;

        public GetCurrentLayout(IRepository<Layout> repository, SiteSettings siteSettings, PageDefaultsSettings pageDefaultsSettings)
        {
            _repository = repository;
            _siteSettings = siteSettings;
            _pageDefaultsSettings = pageDefaultsSettings;
        }

        public Layout Get(Webpage webpage)
        {
            if (webpage != null)
            {
                var layout = GetPageTemplateLayout(webpage);
                if (layout != null) return layout;
                layout = GetTypeDefaultLayout(webpage);
                if (layout != null) return layout;
            }

            return GetSiteDefault();
        }

        private Layout GetTypeDefaultLayout(Webpage webpage)
        {
            var documentMetadata = webpage.GetMetadata();
            var layoutId = _pageDefaultsSettings.GetLayoutId(documentMetadata.Type);
            if (layoutId.HasValue)
            {
                var layout = _repository.GetDataSync(layoutId.Value);
                if (layout != null) return layout;
            }

            return null;
        }

        private static Layout GetPageTemplateLayout(Webpage webpage)
        {
            if (webpage.PageTemplate != null && webpage.PageTemplate.Layout != null) return webpage.PageTemplate.Layout;
            return null;
        }

        private Layout GetSiteDefault()
        {
            var settingValue = _siteSettings.DefaultLayoutId;

            return _repository.GetDataSync<Layout>(settingValue) ??
                   _repository.Readonly().FirstOrDefault();
        }
    }
}