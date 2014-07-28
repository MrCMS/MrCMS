using System.Linq;
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

        public GetCurrentLayout(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public Layout Get(Webpage webpage)
        {
            if (webpage != null)
            {
                if (webpage.PageTemplate != null && webpage.PageTemplate.Layout != null)
                {
                    return webpage.PageTemplate.Layout;
                }
                string defaultLayoutName = webpage.GetMetadata().DefaultLayoutName;
                if (!string.IsNullOrEmpty(defaultLayoutName))
                {
                    Layout layout =
                        _session.QueryOver<Layout>()
                            .Where(x => x.Name == defaultLayoutName)
                            .Cacheable()
                            .List()
                            .FirstOrDefault();
                    if (layout != null)
                        return layout;
                }
            }
            int settingValue = _siteSettings.DefaultLayoutId;

            return _session.Get<Layout>(settingValue) ??
                   _session.QueryOver<Layout>()
                       .Take(1)
                       .Cacheable()
                       .SingleOrDefault();
        }
    }
}