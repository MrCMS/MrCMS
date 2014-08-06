using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class CloneSitePartsService : ICloneSitePartsService
    {
        private readonly ISession _session;
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public CloneSitePartsService(ISession session, ILegacySettingsProvider legacySettingsProvider)
        {
            _session = session;
            _legacySettingsProvider = legacySettingsProvider;
        }

        public void CopySettings(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettingsBases = fromProvider.GetAllSiteSettings();
            siteSettingsBases.ForEach(toProvider.SaveSettings);
        }

        public void CopyLayouts(Site @from, Site to)
        {
            var copies = GetLayoutCopies(@from, to);

            _session.Transact(session => copies.ForEach(layout =>
            {
                session.Save(layout);
                layout.LayoutAreas.ForEach(area =>
                {
                    session.Save(area);
                    area.Widgets.ForEach(widget => session.Save(widget));
                });
            }));
        }

        private IEnumerable<Layout> GetLayoutCopies(Site @from, Site to, Layout fromParent = null, Layout toParent = null)
        {
            var queryOver = _session.QueryOver<Layout>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                            ? queryOver.Where(layout => layout.Parent == null)
                            : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            var layouts = queryOver.List();

            foreach (var layout in layouts)
            {
                var copy = GetCopy(layout, to);
                copy.LayoutAreas = layout.LayoutAreas.Select(area =>
                {
                    var areaCopy = GetCopy(area, to);
                    areaCopy.Layout = copy;
                    areaCopy.Widgets = area.Widgets
                                           .Where(widget => widget.Webpage == null)
                                           .Select(widget =>
                                           {
                                               var widgetCopy = GetCopy(widget, to);
                                               widgetCopy.LayoutArea = areaCopy;
                                               return widgetCopy;
                                           })
                                           .ToList();
                    return areaCopy;
                }).ToList();
                copy.Parent = toParent;
                yield return copy;
                foreach (var child in GetLayoutCopies(@from, to, layout, copy))
                {
                    yield return child;
                }
            }
        }

        private T GetCopy<T>(T entity, Site site) where T : SiteEntity
        {
            var shallowCopy = entity.ShallowCopy();
            shallowCopy.Site = site;
            return shallowCopy;
        }

        public void CopyMediaCategories(Site @from, Site to)
        {
            var copies = GetMediaCategoryCopies(@from, to);

            _session.Transact(session => copies.ForEach(category => session.Save(category)));
        }

        private IEnumerable<MediaCategory> GetMediaCategoryCopies(Site @from, Site to, MediaCategory fromParent = null, MediaCategory toParent = null)
        {
            var queryOver = _session.QueryOver<MediaCategory>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                            ? queryOver.Where(layout => layout.Parent == null)
                            : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            var categories = queryOver.List();
            foreach (var category in categories)
            {
                var copy = GetCopy(category, to);
                copy.Parent = toParent;
                yield return copy;
                foreach (var child in GetMediaCategoryCopies(@from, to, category, copy))
                {
                    yield return child;
                }
            }
        }

        public void CopyHome(Site @from, Site to)
        {
            var home =
                _session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Site.Id == @from.Id && webpage.Parent == null)
                        .OrderBy(webpage => webpage.DisplayOrder)
                        .Asc.Take(1)
                        .SingleOrDefault();

            var copy = GetCopy(home, to);
            _session.Transact(session => session.Save(copy));
        }

        public void Copy404(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error404 = _session.Get<Webpage>(siteSettings.Error404PageId);

            var copy = GetCopy(error404, to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error404PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void Copy403(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error403 = _session.Get<Webpage>(siteSettings.Error403PageId);

            var copy = GetCopy(error403, to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error403PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void Copy500(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error500 = _session.Get<Webpage>(siteSettings.Error500PageId);

            var copy = GetCopy(error500, to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error500PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void CopyLogin(Site @from, Site to)
        {
            var login =
                _session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Site == @from && webpage.DocumentType == "MrCMS.Web.Apps.Core.Pages.LoginPage")
                        .OrderBy(webpage => webpage.DisplayOrder)
                        .Asc.Take(1)
                        .SingleOrDefault();

            if (login != null)
            {
                var loginCopy = GetCopy(login, to);
                _session.Transact(session => session.Save(loginCopy));
            }
        }
    }
}