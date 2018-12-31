using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Models;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class StringResourceAdminService : IStringResourceAdminService
    {
        private const string DefaultLanguage = "Default";
        private readonly IStringResourceProvider _provider;
        private readonly ISession _session;
        private readonly IMapper _mapper;
        private readonly SiteSettings _siteSettings;

        public StringResourceAdminService(IStringResourceProvider provider, SiteSettings siteSettings,
            ISession session, IMapper mapper)
        {
            _provider = provider;
            _siteSettings = siteSettings;
            _session = session;
            _mapper = mapper;
        }

        public IPagedList<StringResource> Search(StringResourceSearchQuery searchQuery)
        {
            IEnumerable<StringResource> resources =
                _provider.AllResources.GetResourcesByKeyAndValue(searchQuery);
            if (searchQuery.Language == DefaultLanguage)
            {
                resources = resources.Where(resource => resource.UICulture == null);
            }
            else if (!string.IsNullOrWhiteSpace(searchQuery.Language))
            {
                resources = resources.Where(resource => resource.UICulture == searchQuery.Language);
            }

            if (searchQuery.SiteId.HasValue)
            {
                if (searchQuery.SiteId == -1)
                {
                    resources = resources.Where(resource => resource.Site == null);
                }
                else if (searchQuery.SiteId > 0)
                {
                    resources =
                        resources.Where(resource => resource.Site != null && resource.Site.Id == searchQuery.SiteId);
                }
            }

            return new PagedList<StringResource>(resources.OrderBy(resource => StringResourceExtensions.GetDisplayKey(resource.Key)), searchQuery.Page,
                _siteSettings.DefaultPageSize);
        }

        public void Add(AddStringResourceModel model)
        {
            var resource = _mapper.Map<StringResource>(model);
            _provider.AddOverride(resource);
        }

        public StringResource GetResource(int id)
        {
            return _session.Get<StringResource>(id);
        }

        public UpdateStringResourceModel GetEditModel(StringResource resource)
        {
            return _mapper.Map<UpdateStringResourceModel>(resource);
        }

        public void Update(UpdateStringResourceModel model)
        {
            var resource = GetResource(model.Id);
            _mapper.Map(model, resource);
            _provider.Update(resource);
        }

        public void Delete(int id)
        {
            var resource = _session.Get<StringResource>(id);
            _provider.Delete(resource);
        }

        public List<SelectListItem> GetLanguageOptions(string key, int? siteId)
        {
            List<CultureInfo> cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            IEnumerable<string> languages = _provider.GetOverriddenLanguages(key, siteId);
            cultureInfos.RemoveAll(info => languages.Contains(info.Name));
            return cultureInfos.OrderBy(info => info.DisplayName)
                .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                    info => info.Name == _siteSettings.UICulture,
                    SelectListItemHelper.EmptyItem("Select a culture..."));
        }

        public List<SelectListItem> SearchLanguageOptions()
        {
            List<CultureInfo> cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            IEnumerable<string> languages = _provider.GetOverriddenLanguages();
            cultureInfos = cultureInfos.FindAll(info => languages.Contains(info.Name));

            List<SelectListItem> selectListItems = cultureInfos.OrderBy(info => info.DisplayName)
                .BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);

            selectListItems.Insert(0, new SelectListItem { Text = "Any", Value = "" });
            selectListItems.Insert(1, new SelectListItem { Text = DefaultLanguage, Value = DefaultLanguage });
            return selectListItems;
        }

        public AddStringResourceModel GetNewResource(string key, int? id)
        {
            string value =
                _provider.AllResources.Where(x => x.Key == key && x.Site == null && x.UICulture == null)
                    .Select(resource => resource.Value)
                    .FirstOrDefault();
            return new AddStringResourceModel { Key = key, SiteId = id, Value = value };
        }

        public List<SelectListItem> ChooseSiteOptions(ChooseSiteParams chooseSiteParams)
        {
            IEnumerable<StringResource> resourcesByKey = _provider.AllResources.Where(x => x.Key == chooseSiteParams.Key);
            List<Site> sites = GetAllSites();

            if (!chooseSiteParams.Language)
            {
                resourcesByKey = resourcesByKey.Where(resource => resource.Site != null && resource.UICulture == null);
                sites =
                    sites.Where(site => !resourcesByKey.Select(resource => resource.Site.Id).Contains(site.Id)).ToList();
            }

            return sites
                .BuildSelectItemList(site => site.DisplayName, site => site.Id.ToString(), emptyItem: null);
        }

        public List<SelectListItem> SearchSiteOptions()
        {
            List<SelectListItem> siteOptions = GetAllSites()
                .BuildSelectItemList(site => site.DisplayName, site => site.Id.ToString(), emptyItemText: "All");

            siteOptions.Insert(1, new SelectListItem
            {
                Text = "System Default",
                Value = "-1"
            });

            return siteOptions;
        }

        private List<Site> GetAllSites()
        {
            return _session.Query<Site>()
                .OrderBy(x => x.Name).WithOptions(x => x.SetCacheable(true)).ToList();
        }
    }
}