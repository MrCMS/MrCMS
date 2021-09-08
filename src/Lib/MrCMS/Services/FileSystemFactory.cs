using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class FileSystemFactory : IFileSystemFactory
    {
        private readonly IConfigurationProviderFactory _configurationProviderFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly ILoggerFactory _loggerFactory;

        public FileSystemFactory(IConfigurationProviderFactory configurationProviderFactory,
            IWebHostEnvironment webHostEnvironment,
            ICurrentSiteLocator currentSiteLocator, ILoggerFactory loggerFactory)
        {
            _configurationProviderFactory = configurationProviderFactory;
            _webHostEnvironment = webHostEnvironment;
            _currentSiteLocator = currentSiteLocator;
            _loggerFactory = loggerFactory;
        }

        public IFileSystem GetForCurrentSite()
        {
            var currentSite = _currentSiteLocator.GetCurrentSite();
            return GetForSite(currentSite);
        }

        public IFileSystem GetForSite(Site site)
        {
            var provider = _configurationProviderFactory.GetForSite(site);

            var settings = provider.GetSiteSettings<FileSystemSettings>();

            var storageType = settings.StorageType;
            if (string.IsNullOrWhiteSpace(storageType))
            {
                return new FileSystem(_webHostEnvironment);
            }

            var type = TypeHelper.GetTypeByName(storageType);
            switch (type?.Name)
            {
                case nameof(FileSystem):
                    return new FileSystem(_webHostEnvironment);
                case nameof(AzureFileSystem):
                    return new AzureFileSystem(provider.GetSiteSettings<FileSystemSettings>(), _loggerFactory.CreateLogger<AzureFileSystem>());
                default:
                    throw new InvalidOperationException($"Cannot find and/or build type '{storageType}'");
            }
        }
    }
}