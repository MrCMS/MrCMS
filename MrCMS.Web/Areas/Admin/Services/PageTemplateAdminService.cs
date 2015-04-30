using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PageTemplateAdminService : IPageTemplateAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly ISession _session;

        public PageTemplateAdminService(ISession session, IGetUrlGeneratorOptions getUrlGeneratorOptions)
        {
            _session = session;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
        }

        public IPagedList<PageTemplate> Search(PageTemplateSearchQuery query)
        {
            IQueryOver<PageTemplate, PageTemplate> queryOver = _session.QueryOver<PageTemplate>();

            return queryOver.Paged(query.Page);
        }

        public void Add(PageTemplate template)
        {
            _session.Transact(session => session.Save(template));
        }

        public void Update(PageTemplate template)
        {
            _session.Transact(session => session.Update(template));
        }

        public List<SelectListItem> GetPageTypeOptions()
        {
            List<SelectListItem> selectListItems = GetNewList();
            Dictionary<Type, string>.KeyCollection appWebpageTypes = MrCMSApp.AppWebpages.Keys;
            selectListItems.AddRange(from key in appWebpageTypes.OrderBy(type => type.FullName)
                let appName = MrCMSApp.AppWebpages[key]
                select
                    new SelectListItem
                    {
                        Text = string.Format("{0} ({1})", key.GetMetadata().Name, appName),
                        Value = key.FullName
                    });
            selectListItems.AddRange(
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .FindAll(type => !appWebpageTypes.Contains(type))
                    .Select(type => new SelectListItem
                    {
                        Text = string.Format("{0} (System)", type.Name.BreakUpString()),
                        Value = type.FullName
                    }));
            return selectListItems;
        }

        public List<SelectListItem> GetLayoutOptions()
        {
            IEnumerable<Layout> layouts =
                _session.QueryOver<Layout>().Where(x => x.Hidden == false).Cacheable().List();

            return layouts.BuildSelectItemList(layout => layout.Name,
                layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                emptyItemText: "Please select...");
        }

        public List<SelectListItem> GetUrlGeneratorOptions(Type type)
        {
            List<SelectListItem> urlGeneratorOptions = _getUrlGeneratorOptions.Get(type);
            urlGeneratorOptions.Insert(0, new SelectListItem {Text = "Please select...", Value = ""});
            return urlGeneratorOptions;
        }

        private static List<SelectListItem> GetNewList()
        {
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "Please select...", Value = ""}
            };
            return selectListItems;
        }
    }
}