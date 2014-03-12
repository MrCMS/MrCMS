using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.Resources
{
    public class StringResourceProvider : IStringResourceProvider
    {
        private readonly ISession _session;
        private readonly Site _site;
        private readonly SiteSettings _siteSettings;
        private static IList<StringResource> _allResources;

        public StringResourceProvider(ISession session, Site site, SiteSettings siteSettings)
        {
            _session = session;
            _site = site;
            _siteSettings = siteSettings;
        }

        public string GetValue(string key, string defaultValue = null)
        {
            var languageValue =
                ResourcesForSite.SingleOrDefault(
                    resource => resource.Key == key && resource.UICulture == _siteSettings.UICulture);
            if (languageValue != null)
                return languageValue.Value;
            var defaultResource =
                ResourcesForSite.SingleOrDefault(resource => resource.Key == key && resource.UICulture == null);
            if (defaultResource == null)
            {
                defaultResource = new StringResource { Key = key, Value = defaultValue ?? key };
                _session.Transact(session => session.Save(defaultResource));
                AllResources.Add(defaultResource);
            }
            return defaultResource.Value;
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

        public void AddOverride(StringResource resource)
        {
            if (resource.UICulture == null)
                return;
            _session.Transact(session => session.Save(resource));
            AllResources.Add(resource);
        }

        public void Update(StringResource resource)
        {
            var firstOrDefault =
                ResourcesForSite.FirstOrDefault(stringResource => stringResource.Id == resource.Id);
            if (firstOrDefault != null) firstOrDefault.Value = resource.Value;
            _session.Transact(session => session.Update(resource));
        }

        public void Delete(StringResource resource)
        {
            var firstOrDefault =
                ResourcesForSite.FirstOrDefault(stringResource => stringResource.Id == resource.Id);
            if (firstOrDefault != null) AllResources.Remove(firstOrDefault);
            _session.Transact(session => session.Delete(resource));
        }

        public IEnumerable<StringResource> ResourcesForSite
        {
            get { return AllResources.Where(resource => resource.Site.Id == _site.Id); }
        }


        private IList<StringResource> AllResources
        {
            get
            {
                return
                    _allResources =
                        _allResources ??
                        GetAllResourcesFromDb();
            }
        }

        private IList<StringResource> GetAllResourcesFromDb()
        {
            return _session.QueryOver<StringResource>().Cacheable().List();
        }
    }
}