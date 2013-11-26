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

        public CloneSitePartsService(ISession session)
        {
            _session = session;
        }

        public void CopySettings(Site @from, Site to)
        {
            _session.Transact(session =>
            {
                var fromProvider = new ConfigurationProvider(new SettingService(_session, @from), @from);
                var toProvider = new ConfigurationProvider(new SettingService(_session, @to), @to);
                var siteSettingsBases = fromProvider.GetAllSiteSettings();
                siteSettingsBases.ForEach(@base =>
                {
                    @base.Site = to;
                    toProvider.SaveSettings(@base);
                });
            });
        }

        public void CopyLayouts(Site @from, Site to)
        {
            var layouts =
                _session.QueryOver<Layout>().Where(layout => layout.Site == @from && layout.Parent == null).List();

            var copies = layouts.Select(layout => CopyLayout(layout, to));

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

        private Layout CopyLayout(Layout layout, Site to)
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
            copy.Children = layout.Children.OfType<Layout>().Select(childLayout =>
            {
                var child = CopyLayout(childLayout, to);
                child.Parent = copy;
                return child;
            }).Cast<Document>().ToList();
            return copy;
        }

        private T GetCopy<T>(T entity, Site site) where T : SiteEntity
        {
            var shallowCopy = entity.ShallowCopy();
            shallowCopy.Site = site;
            return shallowCopy;
        }

        public void CopyMediaCategories(Site @from, Site to)
        {
            var mediaCategories = _session.QueryOver<MediaCategory>().Where(category => category.Site == @from && category.Parent == null).List();

            var copies = mediaCategories.Select(category => CopyMediaCategory(category, to));

            _session.Transact(session => copies.ForEach(category => session.Save(category)));
        }

        private MediaCategory CopyMediaCategory(MediaCategory category, Site to)
        {
            var copy = GetCopy(category, to);
            copy.Children =
                category.Children.OfType<MediaCategory>()
                        .Select(childLayout =>
                        {
                            var child = CopyMediaCategory(childLayout, to);
                            child.Parent = copy;
                            return child;
                        })
                        .Cast<Document>()
                        .ToList();
            return copy;
        }

        public void CopyHome(Site @from, Site to)
        {
            var home =
                _session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Site == @from && webpage.Parent == null)
                        .OrderBy(webpage => webpage.DisplayOrder)
                        .Asc.Take(1)
                        .SingleOrDefault();

            var copy = GetCopy(home, to);
            _session.Transact(session => session.Save(copy));
        }

        public void Copy404(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(new SettingService(_session, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_session, @to), @to);
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
            var fromProvider = new ConfigurationProvider(new SettingService(_session, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_session, @to), @to);
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
            var fromProvider = new ConfigurationProvider(new SettingService(_session, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_session, @to), @to);
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