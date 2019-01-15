using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

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
            var requestServices = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext
                .RequestServices;
            var localizationManager = requestServices.GetRequiredService<ILocalizationManager>();
            var currentCulture = requestServices.GetRequiredService<IGetCurrentUserCultureInfo>();
            var currentSiteLocator = requestServices.GetRequiredService<ICurrentSiteLocator>();
            var records = localizationManager.GetLocalizations();

            return new StringLocalizer(records,
                currentCulture.Get(),
                currentSiteLocator.GetCurrentSite(),
                info => _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.RequestServices
                    .GetRequiredService<ILocalizationManager>().HandleMissingLocalization(info));
        }
    }
}