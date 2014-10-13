using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Filters;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Resources
{
    public class StringResourceProvider : IStringResourceProvider
    {
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IGetCurrentUserCultureInfo _getCurrentUserCultureInfo;
        private static HashSet<StringResource> _allResources;

        public StringResourceProvider(ISession session, Site site, IGetCurrentUserCultureInfo getCurrentUserCultureInfo)
        {
            _session = session;
            _site = site;
            _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
        }


        private static readonly object LockObject = new object();
        private bool _retryingAllResources;

        public string GetValue(string key, string defaultValue = null)
        {
            lock (LockObject)
            {
                var currentUserCulture = _getCurrentUserCultureInfo.GetInfoString();
                var languageValue =
                    ResourcesForSite.FirstOrDefault(
                        resource => resource.Key == key && resource.UICulture == currentUserCulture);
                if (languageValue != null)
                    return languageValue.Value;

                var defaultResource =
                    ResourcesForSite.FirstOrDefault(resource => resource.Key == key && resource.UICulture == null);
                if (defaultResource == null)
                {
                    defaultResource = new StringResource { Key = key, Value = defaultValue ?? key };
                    _session.Transact(session => session.Save(defaultResource));
                    AllResources.Add(defaultResource);
                }
                return defaultResource.Value;
            }
        }

        public IEnumerable<string> GetOverriddenLanguages()
        {
            return ResourcesForSite.Select(resource => resource.UICulture).Where(s => !string.IsNullOrWhiteSpace(s));
        }

        public IEnumerable<string> GetOverriddenLanguages(string key)
        {
            return ResourcesForSite.Where(resource => resource.Key == key)
                .Select(resource => resource.UICulture)
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }

        public void Insert(StringResource resource)
        {
            lock (LockObject)
            {
                _session.Transact(session => session.Save(resource));
                AllResources.Add(resource);
            }
        }

        public void AddOverride(StringResource resource)
        {
            lock (LockObject)
            {
                if (resource.UICulture == null)
                    return;
                _session.Transact(session => session.Save(resource));
                AllResources.Add(resource);
            }
        }

        public void Update(StringResource resource)
        {
            lock (LockObject)
            {
                var firstOrDefault =
                    AllResources.FirstOrDefault(stringResource => stringResource.Id == resource.Id);
                if (firstOrDefault != null) firstOrDefault.Value = resource.Value;
                _session.Transact(session => session.Update(resource));
            }
        }

        public void Delete(StringResource resource)
        {
            lock (LockObject)
            {
                var firstOrDefault =
                    AllResources.FirstOrDefault(stringResource => stringResource.Id == resource.Id);
                if (firstOrDefault != null) AllResources.Remove(firstOrDefault);
                _session.Transact(session => session.Delete(resource));
            }
        }

        public IEnumerable<StringResource> ResourcesForSite
        {
            get { return AllResources.Where(resource => resource.Site.Id == _site.Id); }
        }

        IEnumerable<StringResource> IStringResourceProvider.AllResources
        {
            get { return AllResources; }
        }

        // retry added to try and help mitigate issues with duplicates being added and causing errors
        private HashSet<StringResource> AllResources
        {
            get
            {
                var allSettings = _allResources = _allResources ?? GetAllResourcesFromDb();
                if (!allSettings.Any())
                {
                    try
                    {
                        if (!_retryingAllResources)
                        {
                            CurrentRequestData.ErrorSignal.Raise(new Exception("Settings list empty"));
                        }
                    }
                    catch
                    {

                    }
                    if (!_retryingAllResources)
                    {
                        _retryingAllResources = true;
                        ResetResourceCache();
                        return AllResources;
                    }
                }
                return allSettings;
            }
        }

        private void ResetResourceCache()
        {
            _allResources = null;
        }

        private HashSet<StringResource> GetAllResourcesFromDb()
        {
            using (new SiteFilterDisabler(_session))
            {
                return new HashSet<StringResource>(_session.QueryOver<StringResource>().List());
            }
        }
    }
}