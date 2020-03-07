using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using MrCMS.DbConfiguration;

namespace MrCMS.Services.Resources
{
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        public const string Global = "Global";
        private readonly IServiceProvider _serviceProvider;

        public StringLocalizerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return CreateLocalizer();
        }

        /// <summary>
        ///     This method is used of view and un-typed string localizer building.
        ///     Because of this, we'll use a standard "Global" resource Key
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return CreateLocalizer();
        }

        private IStringLocalizer CreateLocalizer()
        {
            //var requestServices = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext
            //    .RequestServices;
            using (var scope = _serviceProvider.CreateScope())
            {
                ICheckInstallationStatus checkInstallationStatus =
                    scope.ServiceProvider.GetRequiredService<ICheckInstallationStatus>();
                if (!checkInstallationStatus.IsInstalled())
                {
                    return new DefaultLocalizer();
                }

                var requestServices = scope.ServiceProvider;
                var localizationManager = requestServices.GetRequiredService<ILocalizationManager>();
                var currentCulture = requestServices.GetRequiredService<IGetCurrentUserCultureInfo>();
                var currentSiteLocator = requestServices.GetRequiredService<ICurrentSiteLocator>();
                var records = localizationManager.GetLocalizations();

                return new StringLocalizer(records,
                    currentCulture.Get(),
                    currentSiteLocator.GetCurrentSite().GetAwaiter().GetResult(),
                    info => requestServices.GetRequiredService<ILocalizationManager>().HandleMissingLocalization(info));
            }
        }

        private class DefaultLocalizer : IStringLocalizer
        {
            private readonly List<LocalizedString> _strings = new List<LocalizedString>();

            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            {
                return _strings;
            }

            public IStringLocalizer WithCulture(CultureInfo culture)
            {
                return this;
            }

            public LocalizedString this[string name] => new LocalizedString(name, name);

            public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, name);
        }
    }
}