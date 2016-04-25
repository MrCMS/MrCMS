using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PageDefaultsAdminService : IPageDefaultsAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IGetLayoutOptions _getLayoutOptions;
        private readonly ISession _session;
        private readonly IConfigurationProvider _configurationProvider;

        public PageDefaultsAdminService(IConfigurationProvider configurationProvider,
            IGetUrlGeneratorOptions getUrlGeneratorOptions, IGetLayoutOptions getLayoutOptions, ISession session)
        {
            _configurationProvider = configurationProvider;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
            _getLayoutOptions = getLayoutOptions;
            _session = session;
        }


        public List<PageDefaultsInfo> GetAll()
        {
            var layoutOptions = _getLayoutOptions.Get();
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            return (from key in MrCMSApp.AppWebpages.Keys.OrderBy(type => type.FullName)
                    select new PageDefaultsInfo
                    {
                        DisplayName = GetDisplayName(key),
                        TypeName = key.FullName,
                        GeneratorDisplayName = settings.GetGeneratorType(key).Name.BreakUpString(),
                        LayoutName = GetLayoutName(layoutOptions, key)
                    }).ToList();
        }

        private static string GetDisplayName(Type key)
        {
            var appName = MrCMSApp.AppWebpages[key];
            return string.Format("{0} ({1})", key.GetMetadata().Name, appName);
        }

        private string GetLayoutName(List<SelectListItem> layoutOptions, Type key)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var layoutId = settings.GetLayoutId(key);
            if (!layoutId.HasValue || layoutOptions.All(item => item.Value != layoutId.ToString()))
            {
                var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
                var systemDefaultLayout = _session.Get<Layout>(siteSettings.DefaultLayoutId);
                return string.Format("System Default ({0})", systemDefaultLayout.Name);
            }
            return _session.Get<Layout>(layoutId.Value).Name;
        }

        public List<SelectListItem> GetUrlGeneratorOptions(Type type)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var currentGeneratorType = settings.GetGeneratorType(type);
            return _getUrlGeneratorOptions.Get(type, currentGeneratorType);
        }

        public List<SelectListItem> GetLayoutOptions()
        {
            return _getLayoutOptions.Get();
        }

        public DefaultsInfo GetInfo(Type type)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            return new DefaultsInfo
            {
                PageTypeName = type.FullName,
                PageTypeDisplayName = GetDisplayName(type),
                GeneratorTypeName = settings.GetGeneratorType(type).FullName,
                LayoutId = settings.GetLayoutId(type)
            };
        }

        public void SetDefaults(DefaultsInfo info)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.UrlGenerators[info.PageTypeName] = info.GeneratorTypeName;
            settings.Layouts[info.PageTypeName] = info.LayoutId;
            _configurationProvider.SaveSettings(settings);
        }
    }
}