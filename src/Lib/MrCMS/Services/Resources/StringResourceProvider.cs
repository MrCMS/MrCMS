using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services.Resources
{
    public class StringResourceProvider : IStringResourceProvider
    {
        // private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private static Dictionary<string, HashSet<StringResource>> _allResources;

        private static readonly Dictionary<int, Dictionary<string, HashSet<StringResource>>> _resourcesBySite =
            new Dictionary<int, Dictionary<string, HashSet<StringResource>>>();


        private readonly IGetCurrentUserCultureInfo _getCurrentUserCultureInfo;
        private readonly ILogger<StringResourceProvider> _logger;
        private readonly ISession _session;
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private bool _retryingAllResources;

        public StringResourceProvider(ISession session, ICurrentSiteLocator currentSiteLocator,
            IGetCurrentUserCultureInfo getCurrentUserCultureInfo,
            ILogger<StringResourceProvider> logger)
        {
            _session = session;
            _currentSiteLocator = currentSiteLocator;
            _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
            _logger = logger;
        }

        private async Task<IEnumerable<StringResource>> GetAllStringResources()
        {
            return (await GetAllResources()).SelectMany(pair => pair.Value);
        }

        // retry added to try and help mitigate issues with duplicates being added and causing errors
        private async Task<Dictionary<string, HashSet<StringResource>>> GetAllResources()
        {
            var allResources = _allResources ??= await GetAllResourcesFromDb();
            if (!allResources.Any())
            {
                try
                {
                    if (!_retryingAllResources)
                    {
                        _logger.Log(LogLevel.Information, "Resource list empty");
                    }
                }
                catch
                {
                    // ignored
                }

                if (!_retryingAllResources)
                {
                    _retryingAllResources = true;
                    ResetResourceCache();
                    return await GetAllResources();
                }
            }

            return allResources;
        }


        public async Task<Dictionary<string, HashSet<StringResource>>> GetResourcesForCurrentSite()
        {
            var site = _currentSiteLocator.GetCurrentSite();
            if (_resourcesBySite.ContainsKey(site.Id))
                return _resourcesBySite[site.Id];
            else
            {
                var allResources = await GetAllResources();
                return _resourcesBySite[site.Id] =
                    allResources.Keys.ToDictionary(s => s,
                        s =>
                            new HashSet<StringResource>(allResources[s].Where(
                                    resource => resource.Site == null || resource.Site.Id == site.Id)
                                .OrderByDescending(resource => resource.Site != null ? 1 : 0)),
                        StringComparer.OrdinalIgnoreCase);
            }
        }

        public async Task<string> GetValue(string key, string defaultValue = null)
        {
            return await GetValueForCulture(key, await _getCurrentUserCultureInfo.Get(), defaultValue);
        }

        public async Task<string> GetValueForCulture(string key, CultureInfo cultureInfo, string defaultValue = null)
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                string currentUserCulture;
                currentUserCulture = cultureInfo.Name;

                var resourcesForSite = await GetResourcesForCurrentSite();
                if (resourcesForSite.ContainsKey(key))
                {
                    var resources = resourcesForSite[key];

                    var languageValue =
                        resources.FirstOrDefault(
                            resource => resource.UICulture == currentUserCulture);
                    if (languageValue != null)
                        return languageValue.Value;


                    var existingDefault =
                        resources.FirstOrDefault(resource => resource.UICulture == null);
                    if (existingDefault != null)
                        return existingDefault.Value;
                }


                var defaultResource = new StringResource
                {
                    Key = key,
                    Value = defaultValue ?? key,
                    //UICulture = currentUserCulture
                };
                await _session.TransactAsync(session => session.SaveAsync(defaultResource));
                //AllResources[key] = new HashSet<StringResource> {defaultResource};
                ResetResourceCache();
                return defaultResource.Value;
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }

        public async Task<IEnumerable<string>> GetOverriddenLanguages()
        {
            return
                (await GetResourcesForCurrentSite()).SelectMany(x => x.Value)
                .Select(resource => resource.UICulture)
                .Distinct()
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }

        public async Task<IEnumerable<string>> GetOverriddenLanguages(string key, int? siteId)
        {
            var resourcesForSite = await GetResourcesForCurrentSite();
            if (resourcesForSite.ContainsKey(key))
            {
                var stringResources = resourcesForSite[key];
                stringResources = siteId == null
                    ? stringResources.FindAll(resource => resource.Site == null)
                    : stringResources.FindAll(resource => resource.Site != null && resource.Site.Id == siteId);
                return stringResources
                    .Select(resource => resource.UICulture)
                    .Where(s => !string.IsNullOrWhiteSpace(s));
            }

            return new HashSet<string>();
        }

        public async Task Insert(StringResource resource)
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                var existingResource = await _session
                    .Query<StringResource>().FirstOrDefaultAsync(stringResource => stringResource.Key == resource.Key);
                if (existingResource == null)
                    await _session.TransactAsync(session => session.SaveAsync(resource));
                ResetResourceCache();
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }

        public async Task AddOverride(StringResource resource)
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                if (resource.UICulture == null && resource.Site == null)
                    return;
                await _session.TransactAsync(session => session.SaveAsync(resource));
                ResetResourceCache();
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }

        public async Task Update(StringResource resource)
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                await _session.TransactAsync(session => session.UpdateAsync(resource));
                ResetResourceCache();
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }

        public async Task Delete(StringResource resource)
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                await _session.TransactAsync(session => session.DeleteAsync(resource));
                ResetResourceCache();
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }
        //
        // public IStringLocalizer GetLocalizer()
        // {
        // }

        async Task<IEnumerable<StringResource>> IStringResourceProvider.GetAllResources() =>
            await GetAllStringResources();

        public void ClearCache()
        {
            ResetResourceCache();
        }

        private void ResetResourceCache()
        {
            _allResources = null;
            _resourcesBySite.Clear();
        }

        private async Task<Dictionary<string, HashSet<StringResource>>> GetAllResourcesFromDb()
        {
            // await _semaphoreSlim.WaitAsync();
            try
            {
                var allResourcesFromDb = await _session.QueryOver<StringResource>().Cacheable().ListAsync();
                var groupBy =
                    allResourcesFromDb.ToHashSet().GroupBy(resource => resource.Key,
                        StringComparer.OrdinalIgnoreCase);
                return groupBy.ToDictionary(grouping => grouping.Key, grouping => grouping.ToHashSet());
            }
            finally
            {
                // _semaphoreSlim.Release();
            }
        }
    }
}