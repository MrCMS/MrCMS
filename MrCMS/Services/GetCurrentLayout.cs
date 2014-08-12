using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class GetCurrentLayout : IGetCurrentLayout
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly PageDefaultsSettings _pageDefaultsSettings;

        public GetCurrentLayout(ISession session, SiteSettings siteSettings,PageDefaultsSettings pageDefaultsSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
            _pageDefaultsSettings = pageDefaultsSettings;
        }

        public Layout Get(Webpage webpage)
        {
            if (webpage != null)
            {
                var layout = GetPageTemplateLayout(webpage);
                if (layout != null)
                {
                    return layout;
                }
                layout = GetTypeDefaultLayout(webpage);
                if (layout != null)
                {
                    return layout;
                }
            }
            return GetSiteDefault();
        }

        private Layout GetTypeDefaultLayout(Webpage webpage)
        {
            var documentMetadata = webpage.GetMetadata();
            var layoutId = _pageDefaultsSettings.GetLayoutId(documentMetadata.Type);
            if (layoutId.HasValue)
            {
                var layout = _session.Get<Layout>(layoutId);
                if (layout != null)
                {
                    return layout;
                }
            }
            return null;
        }

        private static Layout GetPageTemplateLayout(Webpage webpage)
        {
            if (webpage.PageTemplate != null && webpage.PageTemplate.Layout != null)
            {
                return webpage.PageTemplate.Layout;
            }
            return null;
        }

        private Layout GetSiteDefault()
        {
            int settingValue = _siteSettings.DefaultLayoutId;

            return _session.Get<Layout>(settingValue) ??
                   _session.QueryOver<Layout>()
                       .Take(1)
                       .Cacheable()
                       .SingleOrDefault();
        }
    }
}