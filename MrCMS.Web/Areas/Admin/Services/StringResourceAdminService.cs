using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class StringResourceAdminService : IStringResourceAdminService
    {
        private const string DefaultLanguage = "Default";
        private readonly IStringResourceProvider _provider;
        private readonly SiteSettings _siteSettings;

        public StringResourceAdminService(IStringResourceProvider provider, SiteSettings siteSettings)
        {
            _provider = provider;
            _siteSettings = siteSettings;
        }

        public IPagedList<StringResource> Search(StringResourceSearchQuery searchQuery)
        {
            var resources = _provider.ResourcesForSite.GetResourcesByKeyAndValue(searchQuery);
            if (searchQuery.Language == DefaultLanguage)
            {
                resources = resources.Where(resource => resource.UICulture == null);
            }
            else if (!string.IsNullOrWhiteSpace(searchQuery.Language))
            {
                resources = resources.Where(resource => resource.UICulture == searchQuery.Language);
            }

            return new PagedList<StringResource>(resources.OrderBy(resource => resource.DisplayKey), searchQuery.Page,
                _siteSettings.DefaultPageSize);
        }

        public void Add(StringResource resource)
        {
            _provider.AddOverride(resource);
        }

        public void Update(StringResource resource)
        {
            _provider.Update(resource);
        }

        public void Delete(StringResource resource)
        {
            _provider.Delete(resource);
        }

        public List<SelectListItem> GetLanguageOptions(string key)
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            var languages = _provider.GetOverriddenLanguages(key);
            cultureInfos.RemoveAll(info => languages.Contains(info.Name));
            return cultureInfos.OrderBy(info => info.DisplayName)
                               .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                                                    info => info.Name == _siteSettings.UICulture,
                                                    SelectListItemHelper.EmptyItem("Select a culture..."));
        }

        public List<SelectListItem> SearchLanguageOptions()
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            var languages = _provider.GetOverriddenLanguages();
            cultureInfos = cultureInfos.FindAll(info => languages.Contains(info.Name));

            var selectListItems = cultureInfos.OrderBy(info => info.DisplayName)
                .BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);

            selectListItems.Insert(0, new SelectListItem { Text = "Any", Value = "" });
            selectListItems.Insert(1, new SelectListItem { Text = DefaultLanguage, Value = DefaultLanguage });
            return selectListItems;
        }
    }

    public static class StringResourceSearchExtensions
    {
        public static IEnumerable<StringResource> GetResourcesByKeyAndValue(this IEnumerable<StringResource> resourcesForQuery, StringResourceSearchQuery searchQuery)
        {
            var resources = resourcesForQuery;

            if (!string.IsNullOrWhiteSpace(searchQuery.Key))
            {
                resources =
                    resources.Where(
                        resource => resource.Key.Contains(searchQuery.Key, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Value))
            {
                resources =
                    resources.Where(resource => resource.Value.Contains(searchQuery.Value, StringComparison.OrdinalIgnoreCase));
            }
            return resources;
        }
    }
}