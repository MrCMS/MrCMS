using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class PageDefaultsAdminService : IPageDefaultsAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IGetLayoutOptions _getLayoutOptions;
        private readonly ISession _session;
        private readonly MrCMSAppContext _appContext;
        private readonly IWebpageMetadataService _webpageMetadataService;
        private readonly IConfigurationProvider _configurationProvider;

        public PageDefaultsAdminService(IConfigurationProvider configurationProvider,
            IGetUrlGeneratorOptions getUrlGeneratorOptions, IGetLayoutOptions getLayoutOptions, ISession session,
            MrCMSAppContext appContext, IWebpageMetadataService webpageMetadataService)
        {
            _configurationProvider = configurationProvider;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
            _getLayoutOptions = getLayoutOptions;
            _session = session;
            _appContext = appContext;
            _webpageMetadataService = webpageMetadataService;
        }


        public async Task<List<PageDefaultsInfo>> GetAll()
        {
            var layoutOptions = await _getLayoutOptions.Get();
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var webpages = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>();
            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            var systemDefaultLayout = await _session.GetAsync<Layout>(siteSettings.DefaultLayoutId);
            var defaultLayoutName = $"System Default ({systemDefaultLayout.Name})";
            return (from key in webpages
                select new PageDefaultsInfo
                {
                    DisplayName = GetDisplayName(key),
                    TypeName = key.FullName,
                    GeneratorDisplayName = settings.GetGeneratorType(key).Name.BreakUpString(),
                    LayoutName = GetLayoutName(layoutOptions, key, defaultLayoutName),
                }).ToList();
        }

        private string GetDisplayName(Type key)
        {
            var appName = _appContext.Types.ContainsKey(key) ? _appContext.Types[key].Name : "System";
            return $"{_webpageMetadataService.GetMetadata(key).Name} ({appName})";
        }

        private string GetLayoutName(List<SelectListItem> layoutOptions, Type key, string defaultLayoutName)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var layoutId = settings.GetLayoutId(key);
            if (!layoutId.HasValue || layoutOptions.All(item => item.Value != layoutId.ToString()))
            {
                return defaultLayoutName;
            }

            return _session.Get<Layout>(layoutId.Value).Name;
        }

        public List<SelectListItem> GetUrlGeneratorOptions(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var currentGeneratorType = settings.GetGeneratorType(type);
            return _getUrlGeneratorOptions.Get(type, currentGeneratorType);
        }

        public async Task<List<SelectListItem>> GetLayoutOptions()
        {
            return await _getLayoutOptions.Get();
        }

        public DefaultsInfo GetInfo(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            return new DefaultsInfo
            {
                PageTypeName = type.FullName,
                PageTypeDisplayName = GetDisplayName(type),
                GeneratorTypeName = settings.GetGeneratorType(type).FullName,
                LayoutId = settings.GetLayoutId(type)
            };
        }

        public async Task SetDefaults(DefaultsInfo info)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.UrlGenerators[info.PageTypeName] = info.GeneratorTypeName;
            settings.Layouts[info.PageTypeName] = info.LayoutId;
            await _configurationProvider.SaveSettings(settings);
        }

        public async Task EnableCache(string typeName)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.DisableCaches.Remove(typeName);
            await _configurationProvider.SaveSettings(settings);
        }

        public async Task DisableCache(string typeName)
        {
            var settings = _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.DisableCaches.Add(typeName);
            await _configurationProvider.SaveSettings(settings);
        }
    }
}