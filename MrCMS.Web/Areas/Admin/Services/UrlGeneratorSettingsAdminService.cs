using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class UrlGeneratorSettingsAdminService : IUrlGeneratorSettingsAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly UrlGeneratorSettings _urlGeneratorSettings;

        public UrlGeneratorSettingsAdminService(IConfigurationProvider configurationProvider, UrlGeneratorSettings urlGeneratorSettings,
            IGetUrlGeneratorOptions getUrlGeneratorOptions)
        {
            _configurationProvider = configurationProvider;
            _urlGeneratorSettings = urlGeneratorSettings;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
        }

        public List<UrlGeneratorSettingInfo> GetAll()
        {
            return (from key in MrCMSApp.AppWebpages.Keys.OrderBy(type => type.FullName)
                let appName = MrCMSApp.AppWebpages[key]
                select
                    new UrlGeneratorSettingInfo
                    {
                        DisplayName = string.Format("{0} ({1})", key.GetMetadata().Name, appName),
                        TypeName = key.FullName,
                        GeneratorDisplayName = _urlGeneratorSettings.GetGeneratorType(key.FullName).Name.BreakUpString()
                    }).ToList();
        }

        public List<SelectListItem> GetUrlGeneratorOptions(Type type)
        {
            List<SelectListItem> urlGeneratorOptions = _getUrlGeneratorOptions.Get(type,
                _urlGeneratorSettings.GetGeneratorType(type));
            return urlGeneratorOptions;
        }

        public DefaultGeneratorInfo GetInfo(Type type)
        {
            return new DefaultGeneratorInfo
            {
                PageTypeName = type.FullName,
                GeneratorTypeName = _urlGeneratorSettings.GetGeneratorType(type).FullName
            };
        }

        public void SetDefault(DefaultGeneratorInfo info)
        {
            _urlGeneratorSettings.Defaults[info.PageTypeName] = info.GeneratorTypeName;
            _configurationProvider.SaveSettings(_urlGeneratorSettings);
        }
    }
}