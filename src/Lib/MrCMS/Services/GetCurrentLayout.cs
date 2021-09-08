using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class GetCurrentLayout : IGetCurrentLayout
    {
        private readonly PageDefaultsSettings _pageDefaultsSettings;
        private readonly IDocumentMetadataService _documentMetadataService;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public GetCurrentLayout(ISession session, SiteSettings siteSettings, PageDefaultsSettings pageDefaultsSettings,
            IDocumentMetadataService documentMetadataService)
        {
            _session = session;
            _siteSettings = siteSettings;
            _pageDefaultsSettings = pageDefaultsSettings;
            _documentMetadataService = documentMetadataService;
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

        public Layout GetUserAccountLayout()
        {
            var siteSettingsUserAccountLayoutId = _siteSettings.UserAccountLayoutId;
            return siteSettingsUserAccountLayoutId > 0 ? _session.Get<Layout>(siteSettingsUserAccountLayoutId) : GetSiteDefault();
        }

        private Layout GetTypeDefaultLayout(Webpage webpage)
        {
            var documentMetadata = _documentMetadataService.GetMetadata(webpage);
            var layoutId = _pageDefaultsSettings.GetLayoutId(documentMetadata.Type);
            if (layoutId.HasValue)
            {
                var layout = _session.Get<Layout>(layoutId);
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

            return _session.Get<Layout>(settingValue) ??
                   _session.QueryOver<Layout>()
                       .Take(1)
                       .Cacheable()
                       .SingleOrDefault();
        }
    }
}