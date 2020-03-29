using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Apps;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PageDefaultsAdminService : IPageDefaultsAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IGetLayoutOptions _getLayoutOptions;
        private readonly IRepository<Layout> _repository;
        private readonly MrCMSAppContext _appContext;
        private readonly IConfigurationProvider _configurationProvider;

        public PageDefaultsAdminService(IConfigurationProvider configurationProvider,
            IGetUrlGeneratorOptions getUrlGeneratorOptions, IGetLayoutOptions getLayoutOptions, IRepository<Layout> repository,
            MrCMSAppContext appContext)
        {
            _configurationProvider = configurationProvider;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
            _getLayoutOptions = getLayoutOptions;
            _repository = repository;
            _appContext = appContext;
        }


        public async Task<List<PageDefaultsInfo>> GetAll()
        {
            var layoutOptions = await _getLayoutOptions.Get();
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var webpages = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>();
            List<PageDefaultsInfo> list = new List<PageDefaultsInfo>();
            foreach (var key in webpages)
                list.Add(new PageDefaultsInfo
                {
                    DisplayName = GetDisplayName(key),
                    TypeName = key.FullName,
                    GeneratorDisplayName = settings.GetGeneratorType(key).Name.BreakUpString(),
                    LayoutName = await GetLayoutName(layoutOptions, key),
                    CacheEnabled = key.GetCustomAttribute<WebpageOutputCacheableAttribute>(false) == null
                        ? CacheEnabledStatus.Unavailable
                        : settings.CacheDisabled(key)
                            ? CacheEnabledStatus.Disabled
                            : CacheEnabledStatus.Enabled
                });

            return list;
        }

        private string GetDisplayName(Type key)
        {
            var appName = _appContext.Types.ContainsKey(key) ? _appContext.Types[key].Name : "System";
            return string.Format("{0} ({1})", key.GetMetadata().Name, appName);
        }

        private async Task<string> GetLayoutName(List<SelectListItem> layoutOptions, Type key)
        {
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var layoutId = settings.GetLayoutId(key);
            if (!layoutId.HasValue || layoutOptions.All(item => item.Value != layoutId.ToString()))
            {
                var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
                var systemDefaultLayout = _repository.GetDataSync(siteSettings.DefaultLayoutId);
                return string.Format("System Default ({0})", systemDefaultLayout.Name);
            }
            return _repository.GetDataSync(layoutId.Value).Name;
        }

        public async Task<List<SelectListItem>> GetUrlGeneratorOptions(Type type)
        {
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var currentGeneratorType = settings.GetGeneratorType(type);
            return _getUrlGeneratorOptions.Get(type, currentGeneratorType);
        }

        public async Task<List<SelectListItem>> GetLayoutOptions()
        {
            return await _getLayoutOptions.Get();
        }

        public async Task<DefaultsInfo> GetInfo(Type type)
        {
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
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
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.UrlGenerators[info.PageTypeName] = info.GeneratorTypeName;
            settings.Layouts[info.PageTypeName] = info.LayoutId;
            await _configurationProvider.SaveSettings(settings);
        }

        public async Task EnableCache(string typeName)
        {
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.DisableCaches.Remove(typeName);
            await _configurationProvider.SaveSettings(settings);
        }

        public async Task DisableCache(string typeName)
        {
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            settings.DisableCaches.Add(typeName);
            await _configurationProvider.SaveSettings(settings);
        }
    }
}